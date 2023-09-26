using PdfiumViewer;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace convertPdfToTIFF
{
	class Program
	{
		static void Main(string[] args)
		{

			string directoryPath = args[0];
			string[] pdfFiles = Directory.GetFiles(directoryPath, "*.pdf");

			
			foreach (string pdfFile in pdfFiles)
			{

				string filename = Path.GetFileNameWithoutExtension(pdfFile);
                ConvertPdfToTiff(filename, args[0]);
				MovePdfFile(pdfFile, args[1]);
			}


			
		}

		//Converts PDF file in TIFF multi-frames file
		//Takes path and name of pdf file in parameters
		private static void ConvertPdfToTiff(string fileName, string directoryPath)
        {

			try
			{
				var filePath = directoryPath + "/" + fileName + ".pdf";
				using (var document = PdfDocument.Load(filePath))
				{
					var pageCount = document.PageCount;
					var dpi = 300;
					Image tiff = document.Render(0, dpi, dpi, PdfRenderFlags.CorrectFromDpi);
					ImageCodecInfo encoderInfo = GetEncoderInfo("image/tiff");

					EncoderParameters encoderParams = new EncoderParameters(2);
					EncoderParameter parameter = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionNone);
					encoderParams.Param[0] = parameter;
					parameter = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
					encoderParams.Param[1] = parameter;

					tiff.Save(directoryPath + "/" + fileName + ".tiff", encoderInfo, encoderParams);

					parameter = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionNone);
					encoderParams.Param[0] = parameter;

					parameter = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
					encoderParams.Param[1] = parameter;
					for (int i = 1; i < pageCount; i++)
					{
						Image im = document.Render(i, dpi, dpi, PdfRenderFlags.CorrectFromDpi);
						tiff.SaveAdd(im, encoderParams);
					}
				}
				Console.WriteLine("File " + fileName + " convert from PDF to TIFF");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		//gets encoder info
		//take string object in parameters
		//return ImageCodecInfo array
		private static ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for (j = 0; j < encoders.Length; ++j)
			{
				if (encoders[j].MimeType == mimeType)
					return encoders[j];
			}
			return null;
		}

		//Create repository if not exists and moves PDF file inside
		//Take in parameters the path of the PDF and the path of the repository where it will move
		private static void MovePdfFile(string pdfPath, string destinationFolderPath)
        {
		
			try
			{
				//Create repository if not exists
				Directory.CreateDirectory(destinationFolderPath);

				// Get name of source file
				string fileName = Path.GetFileName(pdfPath);

				// Determine completly path for file in new repository
				string destinationFilePath = Path.Combine(destinationFolderPath, fileName);

				// Move the file in new repository
				File.Move(pdfPath, destinationFilePath);

			}
			catch (Exception ex)
			{
				Console.WriteLine($"A error happened : {ex.Message}");
			}
		
		}

	}
}