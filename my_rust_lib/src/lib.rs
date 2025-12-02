use std::ffi::{CStr, CString};
use std::os::raw::{c_char, c_int, c_uchar};
use std::ptr;

// 为了方便 C# 知道返回图片的具体规格，我们定义一个简单的结构体
// #[repr(C)] 保证 C# 和 Rust 的内存布局一致
#[repr(C)]
pub struct ImageResult {
    pub data_ptr: *mut u8,
    pub data_len: c_int,
    pub width: c_int,
    pub height: c_int,
}

#[unsafe(no_mangle)]
pub extern "C" fn resize_image_keep_aspect(
    path_ptr: *const c_char,
    target_width: u32,
) -> ImageResult {
    // --- A. 初始化一个“空”的错误结果 ---
    let error_result = ImageResult {
        data_ptr: ptr::null_mut(), // 空指针
        data_len: 0,
        width: 0,
        height: 0,
    };

    if path_ptr.is_null() {
        return error_result;
    }

    let c_str = unsafe { CStr::from_ptr(path_ptr) };
    let path = match c_str.to_str() {
        Ok(s) => s,
        Err(_) => return error_result,
    };
    let img = match image::open(path) {
        Ok(i) => i,
        Err(_) => return error_result,
    };

    let resize = img.resize(
        target_width,
        u32::MAX,
        image::imageops::FilterType::Lanczos3,
    );

    let width = resize.width() as c_int;
    let height = resize.height() as c_int;

    let mut raw_pixels = resize.to_rgba8().into_raw();
    let length = raw_pixels.len() as c_int;
    let ptr = raw_pixels.as_mut_ptr();
    std::mem::forget(raw_pixels);

    ImageResult {
        data_ptr: ptr,
        data_len: length,
        width,
        height,
    }
}

/// 释放内存函数 (必不可少)
#[unsafe(no_mangle)]
pub unsafe extern "C" fn free_image_result(ptr: *mut c_uchar, len: c_int) {
    if !ptr.is_null() && len > 0 {
        let _ = Vec::from_raw_parts(ptr, len as usize, len as usize);
    }
}
