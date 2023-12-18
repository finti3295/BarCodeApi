using GdPicture14;

namespace BarcodeApi.Models
{
    public class GdPictureUtils
    {
        public static List<string> GetBarcodesFromImage(string ImageFileName, int dpi = 200)
        {
            using GdPictureImaging oGdPictureImaging = new();
            int imageId = oGdPictureImaging.CreateGdPictureImageFromFile(ImageFileName);
            if (oGdPictureImaging.GetStat() != GdPictureStatus.OK)
            {
                throw new Exception("The image can't be loaded! Error: " + oGdPictureImaging.GetStat().ToString());
            }
            else
            {
                GdPictureStatus picStatus = GetGdPictureStatus(oGdPictureImaging, imageId, out int barcodeCount);
                var res = new List<string>();
                if (picStatus == GdPictureStatus.OK)
                {
                    if (barcodeCount > 0)
                    {
                        for (int i = 1; i <= barcodeCount; i++)
                        {
                            res.Add(oGdPictureImaging.Barcode1DReaderGetBarcodeValue(i));
                        }
                    }
                    oGdPictureImaging.Barcode1DReaderClear();
                }
                oGdPictureImaging.ReleaseGdPictureImage(imageId);
                return res;
            }

        }

        public static List<string> CreateGdPictureImageFromByteArray(byte[] Data)
        {
            using GdPictureImaging oGdPictureImaging = new();
            int imageId = oGdPictureImaging.CreateGdPictureImageFromByteArray(Data);
            if (oGdPictureImaging.GetStat() != GdPictureStatus.OK)
            {
                throw new Exception("The image can't be loaded! Error: " + oGdPictureImaging.GetStat().ToString());
            }
            else
            {
                GdPictureStatus picStatus = GetGdPictureStatus(oGdPictureImaging, imageId, out int barcodeCount);
                var res = new List<string>();
                if (picStatus == GdPictureStatus.OK)
                {
                    if (barcodeCount > 0)
                    {
                        for (int i = 1; i <= barcodeCount; i++)
                        {
                            res.Add(oGdPictureImaging.Barcode1DReaderGetBarcodeValue(i));
                        }
                    }
                    oGdPictureImaging.Barcode1DReaderClear();
                }
                oGdPictureImaging.ReleaseGdPictureImage(imageId);
                return res;
            }

        }
        public static List<string> GetBarcodesFromFile(string stream, int dpi = 200)
        {
            using (var oGdPicturePDF = new GdPicturePDF())
            {
                var docStat = oGdPicturePDF.LoadFromFile(stream);
                if (docStat == GdPictureStatus.OK)
                {
                    int imageID = oGdPicturePDF.RenderPageToGdPictureImage(dpi, false);

                    docStat = oGdPicturePDF.GetStat();
                    if (docStat == GdPictureStatus.OK)
                    {
                        using (GdPictureImaging oGdPictureImaging = new GdPictureImaging())
                        {
                            GdPictureStatus picStatus = GetGdPictureStatus(oGdPictureImaging, imageID, out int barcodeCount);
                            if (picStatus == GdPictureStatus.OK)
                            {
                                if (barcodeCount > 0)
                                {
                                    var res = new List<string>();
                                    // message.Append("Number of barcodes: " + barcodeCount.ToString());
                                    for (int i = 1; i <= barcodeCount; i++)
                                    {
                                        res.Add(oGdPictureImaging.Barcode1DReaderGetBarcodeValue(i));
                                    }
                                    return res;
                                }

                                oGdPictureImaging.Barcode1DReaderClear();
                            }
                            oGdPictureImaging.ReleaseGdPictureImage(imageID);
                        }
                    }
                }


            }
            return new List<string>();
        }

        public static List<string> GetBarcodesFromImage(Stream stream, int dpi = 200)
        {
            using (var oGdPicturePDF = new GdPicturePDF())
            {
                var docStat = oGdPicturePDF.LoadFromStream(stream);
                if (docStat == GdPictureStatus.OK)
                {
                    int imageID = oGdPicturePDF.RenderPageToGdPictureImage(dpi, false);

                    docStat = oGdPicturePDF.GetStat();
                    if (docStat == GdPictureStatus.OK)
                    {
                        using (GdPictureImaging oGdPictureImaging = new GdPictureImaging())
                        {
                            GdPictureStatus picStatus = GetGdPictureStatus(oGdPictureImaging, imageID, out int barcodeCount);
                            if (picStatus == GdPictureStatus.OK)
                            {
                                if (barcodeCount > 0)
                                {
                                    var res = new List<string>();
                                    // message.Append("Number of barcodes: " + barcodeCount.ToString());
                                    for (int i = 1; i <= barcodeCount; i++)
                                    {
                                        res.Add(oGdPictureImaging.Barcode1DReaderGetBarcodeValue(i));
                                    }
                                    return res;
                                }

                                oGdPictureImaging.Barcode1DReaderClear();
                            }
                            oGdPictureImaging.ReleaseGdPictureImage(imageID);
                        }
                    }
                }


            }
            return new List<string>();
        }

        private static GdPictureStatus GetGdPictureStatus(GdPictureImaging oGdPictureImaging, int imageID,
       out int barcodeCount, int ExpectedCount = 0, bool StopOnExpectedCount = false)
        {
            var barcodeType = Barcode1DReaderType.Barcode1DReaderCode39 | Barcode1DReaderType.Barcode1DReaderCode128;

            var res = oGdPictureImaging.Barcode1DReaderDoScan(imageID, Barcode1DReaderScanMode.BestQuality, barcodeType, false, 0);
            barcodeCount = oGdPictureImaging.Barcode1DReaderGetBarcodeCount();
            return res;

        }

        public enum MyBarcodeReaderType
        {
            DATA_MATRIX = -3,
            QR_CODE = -2,
            PDF_417 = -1,
            //
            // Summary:
            //     None of them.
            Barcode1DReaderNone = 0,
            //
            // Summary:
            //     Industrial 2 of 5
            Barcode1DReaderIndustrial2of5 = 1,
            //
            // Summary:
            //     Inverted 2 of 5
            Barcode1DReaderInverted2of5 = 2,
            //
            // Summary:
            //     Interleaved 2 of 5
            Barcode1DReaderInterleaved2of5 = 4,
            //
            // Summary:
            //     Iata 2 of 5
            Barcode1DReaderIata2of5 = 8,
            //
            // Summary:
            //     Matrix 2 of 5
            Barcode1DReaderMatrix2of5 = 16,
            //
            // Summary:
            //     Code 39
            Barcode1DReaderCode39 = 32,
            //
            // Summary:
            //     Codeabar
            Barcode1DReaderCodeabar = 64,
            //
            // Summary:
            //     Bcd Matrix
            Barcode1DReaderBcdMatrix = 128,
            //
            // Summary:
            //     DataLogic 2 of 5
            Barcode1DReaderDataLogic2of5 = 256,
            //
            // Summary:
            //     Code 128
            Barcode1DReaderCode128 = 4096,
            //
            // Summary:
            //     Code 93
            Barcode1DReaderCODE93 = 16384,
            //
            // Summary:
            //     EAN 13
            Barcode1DReaderEAN13 = 32768,
            //
            // Summary:
            //     UPC Version A
            Barcode1DReaderUPCA = 65536,
            //
            // Summary:
            //     EAN 8
            Barcode1DReaderEAN8 = 131072,
            //
            // Summary:
            //     UPC Version E
            Barcode1DReaderUPCE = 262144,
            //
            // Summary:
            //     ADD 5
            Barcode1DReaderADD5 = 524288,
            //
            // Summary:
            //     ADD 2
            Barcode1DReaderADD2 = 1048576
        }
    }
}
