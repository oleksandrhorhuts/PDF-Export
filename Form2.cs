using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using static System.Windows.Forms.ImageList;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using RasterEdge.WDP;
using RasterEdge.XDoc.PDF.HTML5Editor;


namespace itext
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PrintReceipt();
        }

        #region dims
        const int AlignLeft = 0; const int AlignRight = 1; const int AlignCenter = 2; const int AlignDecimal = 3;
        int intLine; int sngFontSize = 10; int intDataStart; int intDataStop; float sngPageHeight; Single sngPageWidth; Single sngDataAreaHeight; Single sngDataAreaWidth;
        int ItemCount = 0; int ItemsPerPage = 0;
        bool boolPrintHeader = false;
        int zero = 0;
        Single sngHeaderMargin = 18; Single[] pdfColumn = new float[16];
        string strPath; string strLorP = "P"; Single sngLeftMargin = 18; Single sngRightMargin = 18; Single sngTopMargin = 36; Single sngBottomMargin = 36;
        byte[] bData = null; string strNine = ""; string strCourse = ""; bool boolCaddies = false;
        string strReportHeaderDate = DateTime.Now.ToString("dd MMM yyyy hh:mm tt"); int intExtraLines = 0; bool boolCircleGroups = false;
        string strReport = ""; //string strReportDate = DateTime.Now.ToString("dd MMM yyyy hh:mm tt");
        BaseFont pdfBaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        BaseFont pdfBaseFontBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        iTextSharp.text.Document pdfDoc;
        PdfWriter pdfWriter;
        iTextSharp.text.pdf.PdfContentByte pdfCB;
        string inputFileName;
        string outputFilePath;
        Single rowCount;
        
        #endregion

        public void PrintReceipt()
        {

            iTextSharp.text.Rectangle pagesize;
            StringBuilder text = new StringBuilder();
            PdfReader reader = new PdfReader(strPath);

            pagesize = reader.GetPageSize(1);
            sngPageWidth = pagesize.Width + 70;
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            TextMarginFinder finder;
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                finder = parser.ProcessContent(i, new TextMarginFinder());
                sngPageHeight = finder.GetLly();
                
            }
            reader.Close();

            //  open the file
            RasterEdge.XDoc.PDF.PDFDocument doc = new RasterEdge.XDoc.PDF.PDFDocument(strPath);
            //  get the 2nd page
            RasterEdge.XDoc.PDF.PDFPage page = (RasterEdge.XDoc.PDF.PDFPage)doc.GetPage(0);
            //  set crop region: start point (100, 100), width = 300, height = 400
            RectangleF cropArea = new RectangleF(0, 0, sngPageWidth, sngPageHeight);
            //  crop the page
            page.Crop(cropArea);

            //  output the new document
            doc.Save(outputFilePath);

            MessageBox.Show("Done");

        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                 strPath = openFileDialog1.FileName;
                string[] arrAllFiles = openFileDialog1.FileNames; //used when Multiselect = true
                 inputFileName = System.IO.Path.GetFileName(strPath);
                rowCount = GetPdfRowCount(strPath);
                //MessageBox.Show($"Number of rows in the PDF file: {rowCount}");

            }
        }
        private int GetPdfRowCount(string pdfFilePath)
        {
            try
            {
                using (PdfReader pdfReader = new PdfReader(pdfFilePath))
                {
                    int pageCount = pdfReader.NumberOfPages;
                    int rowCount = 0;

                    for (int page = 1; page <= pageCount; page++)
                    {
                        string text = PdfTextExtractor.GetTextFromPage(pdfReader, page);
                        rowCount += text.Split('\n').Length;
                    }

                    return rowCount;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading PDF file: {ex.Message}");
                return -1; // Or handle the error in a way that suits your application
            }
        }

        private void btn_open_folder_Click(object sender, EventArgs e)
        {
            // Check if inputFileName is empty
            if (string.IsNullOrEmpty(inputFileName))
            {
                // Optionally display a message or perform other actions
                MessageBox.Show("Input file name is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the function
            }
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.Description = "Custom Description"; //not mandatory
            string sSelectedFolder;
            if (fbd.ShowDialog() == DialogResult.OK)
                sSelectedFolder = fbd.SelectedPath;
            else
                sSelectedFolder = string.Empty;

            outputFilePath = System.IO.Path.Combine(sSelectedFolder, inputFileName);

        }
    }
}
