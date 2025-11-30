use image::imageops::FilterType;
use std::ffi::{CStr, CString};
use std::os::raw::{c_char, c_int, c_uchar};
use std::ptr;
use std::ptr::null_mut;
use std::time::Instant;
use my_rust_lib::{resize_image_keep_aspect, free_image_result};

fn main() {
    println!("Hello, world!");

    let path = "D:\\FreightStation\\pictures\\beauti\\415461.jpg";
    let c_path = CString::new(path).unwrap();

    let start = Instant::now();
    let img = resize_image_keep_aspect(c_path.as_ptr(), 400);
    let duration = start.elapsed();

    println!("--------------------------------");
    println!("计算耗时: {:?}", duration); // 自动格式化 (如 "12.3ms" 或 "450µs")
    println!("精确毫秒: {} ms", duration.as_millis()); // 只看毫秒整数
    println!("精确微秒: {} µs", duration.as_micros()); // 只看微秒整数
    println!("--------------------------------");

    println!("width:{}, height:{}", img.width, img.height);

    if !img.data_ptr.is_null() {
        unsafe {
            free_image_result(img.data_ptr, img.data_len);
        }
    }
}
