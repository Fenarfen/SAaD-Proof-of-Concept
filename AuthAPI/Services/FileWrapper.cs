using AuthAPI.Interfaces;

namespace AuthAPI.Services;

public class FileWrapper : IFileWrapper
{
	public string ReadAllText(string path) => File.ReadAllText(path);
}
