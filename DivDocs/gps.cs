using System;
using System.Drawing;
using System.Drawing.Imaging;

class Program
{
    static void Main()
    {
        string imagePath = @"C:\path\to\your\image.jpg"; // Replace with your image path

        using (Image image = Image.FromFile(imagePath))
        {
            // Property IDs for GPS Latitude and Longitude
            int[] gpsLatitudePropIds = { 0x0002, 0x0001 }; // GPSLatitude and GPSLatitudeRef
            int[] gpsLongitudePropIds = { 0x0004, 0x0003 }; // GPSLongitude and GPSLongitudeRef

            // Retrieve GPS coordinates
            double? latitude = GetGpsCoordinate(image, gpsLatitudePropIds);
            double? longitude = GetGpsCoordinate(image, gpsLongitudePropIds);

            if (latitude.HasValue && longitude.HasValue)
            {
                Console.WriteLine($"Latitude: {latitude.Value}, Longitude: {longitude.Value}");
            }
            else
            {
                Console.WriteLine("No GPS metadata found in the image.");
            }
        }
    }

    static double? GetGpsCoordinate(Image image, int[] propertyIds)
    {
        try
        {
            PropertyItem propRef = image.GetPropertyItem(propertyIds[1]); // LatitudeRef or LongitudeRef
            PropertyItem propValue = image.GetPropertyItem(propertyIds[0]); // Latitude or Longitude

            string refValue = System.Text.Encoding.ASCII.GetString(propRef.Value).Trim('\0');
            bool isNegative = refValue == "S" || refValue == "W";

            double[] gpsParts = DecodeGpsProperty(propValue.Value);
            if (gpsParts == null) return null;

            double coordinate = gpsParts[0] + gpsParts[1] / 60.0 + gpsParts[2] / 3600.0;
            return isNegative ? -coordinate : coordinate;
        }
        catch
        {
            return null;
        }
    }

    static double[] DecodeGpsProperty(byte[] value)
    {
        try
        {
            // Each GPS part (degrees, minutes, seconds) is stored as a rational number (numerator/denominator)
            double[] gpsParts = new double[3];
            for (int i = 0; i < gpsParts.Length; i++)
            {
                int numerator = BitConverter.ToInt32(value, 8 * i);
                int denominator = BitConverter.ToInt32(value, 8 * i + 4);
                gpsParts[i] = (double)numerator / denominator;
            }
            return gpsParts;
        }
        catch
        {
            return null;
        }
    }
}
