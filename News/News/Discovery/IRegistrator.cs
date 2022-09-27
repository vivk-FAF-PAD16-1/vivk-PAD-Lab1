using System.Threading.Tasks;

namespace News.Discovery
{
	public interface IRegistrator
	{
		Task<bool> Register();
	}
}