use rayon::prelude::*;
use std::ffi::CString;
use std::sync::mpsc::channel;
use std::thread;

// 假设这是你已经实现了 Send 的结构体
struct ImageResult {
    data: *mut u8,
    length: usize,
}
unsafe impl Send for ImageResult {} // 你的手动实现

fn main() {
    // 1. 创建一个通道
    // sender: 用来发任务
    // receiver: 用来接任务
    let (sender, receiver) = channel();

    // 2. 启动处理管线 (Rayon 部分)
    // 注意：这里必须放在单独的线程，或者是主线程的最后
    // 因为 par_bridge 是阻塞的，它会一直等到通道关闭
    let handle = thread::spawn(move || {
        // receiver 是串行的，但 par_bridge() 会把它变成并行的迭代器！
        // Rayon 会自动从通道里抢任务，分发给多个线程
        receiver.into_iter().par_bridge().for_each(|path: String| {
            // --- 这里是并行的 ---
            let c_path = CString::new(path).unwrap();

            // 调用 C 处理
            // let result = unsafe { resize_image_keep_aspect(c_path.as_ptr(), 400) };
            let result = 1;
            // 处理结果：
            // 因为这是流式的，你不能像之前那样 collect() 成一个 Vec
            // 通常你有两个选择：
            // A. 直接存盘
            // save_to_disk(result);

            // B. 发送到另一个通道（结果通道）给下游
            // result_sender.send(result).unwrap();

            println!("Processed image on thread: {:?}", thread::current().id());

            // 别忘了释放 C 的内存，如果需要的话
        });
    });

    // 3. 模拟密集的路径输入 (生产者)
    // 你的业务代码在这里，也许是一个循环，或者是文件监听器
    for i in 0..100 {
        let path = format!("/tmp/image_{}.jpg", i);
        // 把路径塞进通道，send 是非常快的，不会阻塞
        sender.send(path).unwrap();
    }

    // 4. 重要：发送完毕后，必须丢弃 sender
    // 这样 receiver 才知道“不会再有新数据了”，par_bridge 才会停止循环
    drop(sender);

    // 等待处理完成
    handle.join().unwrap();
}

#[cfg(test)]
mod tests {
    // 这一行很重要：引入父模块的所有内容，
    // 这样你才能在测试里调用上面的 add 或 is_even
    use super::*;

    #[test]
    fn test_add_works() {
        main()
    }
}
