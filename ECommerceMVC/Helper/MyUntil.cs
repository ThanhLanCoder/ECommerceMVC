namespace ECommerceMVC.Helper
{
    public class MyUntil
    {
        public static string UploadImage(IFormFile Hinh, string folder)
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
                using (var stream = new FileStream(path, FileMode.CreateNew))
                {
                    Hinh.CopyTo(stream);
                }
                return Hinh.FileName;
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }
    }
}
