# RapidOcrOnnxCs (WPF Version)

### 联系方式

[QQ群](https://rapidai.github.io/RapidOCRDocs/main/communicate/#qq)

### 介绍
* 本项目为Windows平台C# WPF范例。
* 依赖的包：Emgu.CV、MicroSoft.ML.OnnxRuntime、Clipper2

### Demo下载(win、mac、linux)
编译好的demo文件比较大，可以到Q群共享内下载

### 编译环境
1. Windows 10 x64 或更高版本
2. Visual Studio 2022 或以上（支持.NET 8）
3. .NET 8 SDK

### 编译说明
1. 使用Visual Studio 2022打开RapidOcrOnnxCs.sln。
2. 解决方案资源管理器->OcrLib->右键->管理NuGet程序包->浏览->搜索->安装
   * 注意：Emgu.CV要选作者是“Emgu Corporation”
   * Emgu.CV 最新稳定版（兼容.NET 8）
   * Emgu.CV.runtime.windows 最新稳定版（兼容.NET 8）
   * MicroSoft.ML.OnnxRuntime 最新稳定版（兼容.NET 8）
   * Clipper2 最新稳定版
3. 解决方案资源管理器->OcrOnnxWpf->右键->管理NuGet程序包->浏览->搜索->安装
   * 注意：Emgu.CV要选作者是“Emgu Corporation”
   * Emgu.CV 最新稳定版（兼容.NET 8）
   * Emgu.CV.runtime.windows 最新稳定版（兼容.NET 8）
4. 确保：OcrOnnxWpf设为启动项目
5. 确保：OcrOnnxWpf->右键->属性->生成->平台目标:x64
6. 确保：OcrLib->右键->属性->生成->平台目标:x64
7. 生成解决方案
8. 把models文件夹复制到`\RapidOcrOnnxCs\OcrOnnxWpf\bin\Debug\net8.0-windows(或Release\net8.0-windows)`
   * [模型下载地址](https://github.com/znsoftm/BaiPiaoOCR/tree/main/models)
9. 运行

### 技术说明
* 项目使用.NET 8 WPF框架开发
* 采用SDK-style项目文件格式
* 使用WriteableBitmap实现Emgu.CV Mat对象到WPF BitmapSource的转换
* 移除了System.Windows.Forms依赖，完全使用WPF控件和API
* 优化了UI布局，采用Grid布局管理器实现响应式设计

### 功能说明
* 支持加载本地图片文件
* 支持OCR文字识别
* 支持调整识别参数
* 显示详细识别结果和文本结果
* 支持显示分割图和调试模式

### 其它
* 修改模型路径，模型名称，线程数，必须“重新初始化”才能生效
* 输入参数说明请参考[RapidOcrOnnx](https://github.com/RapidAI/RapidOcrOnnx/tree/61d7b434d2b773eb61dab85328240789f69b3ae0#%E8%BE%93%E5%85%A5%E5%8F%82%E6%95%B0%E8%AF%B4%E6%98%8E)
