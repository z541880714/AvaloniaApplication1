use rayon::prelude::*;
use std::fs;
use std::path::Path;
use walkdir::WalkDir;

/// 使用 #[repr(C)] 养成好习惯，虽然 String 不能直接传给 C# (需要转指针)，
/// 但这里的 u64 和 bool 是标准的，C# 可以直接识别。
#[repr(C)]
#[derive(Debug)]
pub struct FileProcessResult {
    pub file_path: String,
    pub file_size: u64,
    pub is_success: bool,
    pub message: String,
}

pub fn scan_and_process(root_dir: &str) -> Vec<FileProcessResult> {
    println!("Scanning... 开始扫描文件夹: {}", root_dir);

    let entries: Vec<_> = WalkDir::new(root_dir)
        .into_iter()
        .filter_map(|e| e.ok())
        .filter(|e| e.file_type().is_file()) // 只保留文件，不要文件夹
        .filter(|e| e.file_name().to_str().unwrap().ends_with(".jpg"))
        .collect();

    println!(
        ">>> 阶段1完成: 找到 {} 个文件。开始并发执行...",
        entries.len()
    );

    let results: Vec<FileProcessResult> = entries
        .par_iter()
        .map(|x| execute_task_on_file(x.path()))
        .collect();
    println!("result len: {}", results.len());

    results
}

/// 这是一个纯 CPU 密集型或混合型的任务函数
/// Rayon 会在多个线程中并行调用它
pub fn execute_task_on_file(path: &Path) -> FileProcessResult {
    let path_str = path.to_str().unwrap().to_string();
    let path_str1 = path.to_string_lossy().to_string();

    match fs::metadata(path) {
        Ok(meta) => FileProcessResult {
            file_path: path_str,
            file_size: meta.len(),
            is_success: true,
            message: "OK".to_string(),
        },

        Err(e) => {
            // 模拟失败情况
            FileProcessResult {
                file_path: path_str,
                file_size: 0,
                is_success: false,
                message: format!("读取失败: {}", e),
            }
        }
    }
}

// -----------------------------------------
// 单元测试区域
// -----------------------------------------
#[cfg(test)]
mod tests {
    // 这一行很重要：引入父模块的所有内容，
    // 这样你才能在测试里调用上面的 add 或 is_even
    use super::*;

    #[test]
    fn test_add_works() {
        let image_dir = Path::new("D:\\__4_scrawl\\images");
        let result = scan_and_process(image_dir.to_str().unwrap());
        for it in &result {
            println!("{:?}", it);
        }
        println!("result size: {}", result.len());
    }
}
