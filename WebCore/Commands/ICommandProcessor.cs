using System.Threading.Tasks;
using System.Windows.Input;

namespace WebCore.Commands
{
    public interface ICommandProcessor
    {
        Task ProcessAsync<T>(T command)
            where T : ICommand;
    }
}