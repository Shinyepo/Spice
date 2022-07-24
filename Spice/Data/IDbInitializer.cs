using System.Threading.Tasks;

namespace Spice.Data
{
    public interface IDbInitializer
    {
        Task Initialize();
    }
}
