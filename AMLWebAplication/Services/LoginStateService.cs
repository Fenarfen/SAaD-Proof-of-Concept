namespace AMLWebAplication.Services
{
	public class LoginStateService
	{
		public event Action OnChange;

		public void NotifyLoginStateChanged()
		{
			OnChange?.Invoke();
		}
	}
}
