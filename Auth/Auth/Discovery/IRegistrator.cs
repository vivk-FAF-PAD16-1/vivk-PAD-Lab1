using System.Threading.Tasks;

namespace Auth.Discovery
{
	public interface IRegistrator
	{
		Task<bool> Register();
	}
}