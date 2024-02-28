public class ImageDownloader
{
    private readonly HttpClient _client = new HttpClient
    {
        MaxResponseContentBufferSize = 1_000_000_000
    };

    public event Action<string, int>? DownloadCompleted;
    public event Action<string>? DownloadStarted;

    public async Task<bool> DownloadAsync(string URL, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            this.DownloadStarted?.Invoke(URL);

            HttpResponseMessage response = await _client.GetAsync(URL, cancellationToken);
            byte[] content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            using var fs = new FileStream(Path.Combine(Environment.CurrentDirectory, fileName), FileMode.Create);
            await response.Content.CopyToAsync(fs, cancellationToken);

            this.DownloadCompleted?.Invoke(fileName, content.Length);
            return true;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Загрузка файла {URL} в {fileName} отменена");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке: {ex.Message}");
            return false;
        }
        finally
        {
            this._client.Dispose();
        }
    }
}