using Emgu.CV;
using Emgu.CV.CvEnum;
using OcrLiteLib;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Windows.Media;

namespace OcrOnnxWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OcrLite? ocrEngin;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string modelsDir = appPath + "models";
            modelsTextBox.Text = modelsDir;

            // ch_PP-OCRv4_det_infer.onnx
            // ch_ppocr_mobile_v2.0_cls_infer.onnx
            // ch_PP-OCRv4_rec_infer.onnx
            // ppocr_keys_v1.txt
            detNameTextBox.Text = "ch_PP-OCRv4_det_infer.onnx";
            clsNameTextBox.Text = "ch_ppocr_mobile_v2.0_cls_infer.onnx";
            recNameTextBox.Text = "ch_PP-OCRv4_rec_infer.onnx";
            keysNameTextBox.Text = "ppocr_keys_v1.txt";

            // 默认选中一些选项
            doAngleCheckBox.IsChecked = true;
            mostAngleCheckBox.IsChecked = true;
        }

        private void initBtn_Click(object sender, RoutedEventArgs e)
        {
            InitializeOcrEngine();
        }

        private void InitializeOcrEngine()
        {
            string modelsDir = modelsTextBox.Text;
            string detPath = modelsDir + "\\" + detNameTextBox.Text;
            string clsPath = modelsDir + "\\" + clsNameTextBox.Text;
            string recPath = modelsDir + "\\" + recNameTextBox.Text;
            string keysPath = modelsDir + "\\" + keysNameTextBox.Text;

            bool isDetExists = File.Exists(detPath);
            if (!isDetExists)
            {
                MessageBox.Show("模型文件不存在:" + detPath);
                return;
            }
            bool isClsExists = File.Exists(clsPath);
            if (!isClsExists)
            {
                MessageBox.Show("模型文件不存在:" + clsPath);
                return;
            }
            bool isRecExists = File.Exists(recPath);
            if (!isRecExists)
            {
                MessageBox.Show("模型文件不存在:" + recPath);
                return;
            }
            bool isKeysExists = File.Exists(keysPath);
            if (!isKeysExists)
            {
                MessageBox.Show("Keys文件不存在:" + keysPath);
                return;
            }

            try
            {
                ocrEngin = new OcrLite();
                ocrEngin.InitModels(detPath, clsPath, recPath, keysPath, 4); // 默认4线程
                MessageBox.Show("初始化成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化失败: " + ex.Message);
            }
        }

        private void openBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件 (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件 (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                pathTextBox.Text = openFileDialog.FileName;
                LoadImage(openFileDialog.FileName);
            }
        }

        private void LoadImage(string filePath)
        {
            try
            {
                Mat src = CvInvoke.Imread(filePath, ImreadModes.Color);
                imageControl.Source = MatToBitmapSource(src);
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载图片失败: " + ex.Message);
            }
        }

        private void modelsBtn_Click(object sender, RoutedEventArgs e)
        {
            // 使用WPF兼容的文件夹选择对话框
            var dialog = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "选择文件夹",
                Filter = "文件夹|*.folder"
            };
            
            if (dialog.ShowDialog() == true)
            {
                string folderPath = Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(folderPath))
                {
                    modelsTextBox.Text = folderPath;
                }
            }
        }

        private void detectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ocrEngin == null)
            {
                MessageBox.Show("未初始化，无法执行!");
                return;
            }

            string targetImg = pathTextBox.Text;
            if (!File.Exists(targetImg))
            {
                MessageBox.Show("目标图片不存在，请用Open按钮打开");
                return;
            }

            try
            {
                // 获取参数
                int padding = 50; // 默认值
                int imgResize = 960; // 默认值
                float boxScoreThresh = 0.5f; // 默认值
                float boxThresh = 0.3f; // 默认值
                float unClipRatio = 2.0f; // 默认值

                // 解析参数（实际项目中应该添加输入验证）
                if (int.TryParse(paddingNumeric.Text, out int parsedPadding))
                    padding = parsedPadding;
                if (int.TryParse(imgResizeNumeric.Text, out int parsedResize))
                    imgResize = parsedResize;
                if (float.TryParse(boxScoreThreshTextBox.Text, out float parsedBoxScore))
                    boxScoreThresh = parsedBoxScore;
                if (float.TryParse(boxThreshTextBox.Text, out float parsedBoxThresh))
                    boxThresh = parsedBoxThresh;
                if (float.TryParse(unClipRatioTextBox.Text, out float parsedUnClip))
                    unClipRatio = parsedUnClip;

                bool doAngle = doAngleCheckBox.IsChecked == true;
                bool mostAngle = mostAngleCheckBox.IsChecked == true;

                // 执行OCR检测
                OcrResult ocrResult = ocrEngin.Detect(targetImg, padding, imgResize, boxScoreThresh, boxThresh, unClipRatio, doAngle, mostAngle);

                // 显示结果
                ocrResultTextBox.Text = ocrResult.ToString();
                strRestTextBox.Text = ocrResult.StrRes;
                imageControl.Source = MatToBitmapSource(ocrResult.BoxImg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("检测失败: " + ex.Message);
            }
        }

        private void partImgCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ocrEngin != null)
                ocrEngin.isPartImg = true;
        }

        private void partImgCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ocrEngin != null)
                ocrEngin.isPartImg = false;
        }

        private void debugCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ocrEngin != null)
                ocrEngin.isDebugImg = true;
        }

        private void debugCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ocrEngin != null)
                ocrEngin.isDebugImg = false;
        }

        // 将Emgu.CV的Mat对象转换为WPF的BitmapSource
        private BitmapSource? MatToBitmapSource(Mat mat)
        {
            if (mat == null || mat.IsEmpty)
                return null;

            try
            {
                // 确保Mat是RGB格式（WPF使用RGB，Emgu.CV默认是BGR）
                Mat rgbMat = new Mat();
                if (mat.NumberOfChannels == 1) // 灰度图
                {
                    CvInvoke.CvtColor(mat, rgbMat, ColorConversion.Gray2Bgr);
                    CvInvoke.CvtColor(rgbMat, rgbMat, ColorConversion.Bgr2Rgb);
                }
                else if (mat.NumberOfChannels == 3) // BGR转换为RGB
                {
                    CvInvoke.CvtColor(mat, rgbMat, ColorConversion.Bgr2Rgb);
                }
                else if (mat.NumberOfChannels == 4) // RGBA转换为RGB
                {
                    CvInvoke.CvtColor(mat, rgbMat, ColorConversion.Rgba2Rgb);
                }
                else
                {
                    rgbMat = mat;
                }

                // 创建WriteableBitmap
                WriteableBitmap bitmap = new WriteableBitmap(
                    rgbMat.Width,
                    rgbMat.Height,
                    96, // DPI X
                    96, // DPI Y
                    PixelFormats.Rgb24,
                    null);

                // 复制数据到WriteableBitmap
                int bufferSize = rgbMat.Total.ToInt32() * rgbMat.NumberOfChannels;
                bitmap.WritePixels(
                    new Int32Rect(0, 0, rgbMat.Width, rgbMat.Height),
                    rgbMat.DataPointer,
                    bufferSize,
                    rgbMat.Step);

                return bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("图像转换失败: " + ex.Message);
                return null;
            }
        }

        // 导入GDI32.dll，用于释放hBitmap
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
    }
}