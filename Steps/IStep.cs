
using System;
using System.Threading.Tasks;

namespace Steps
{
  public interface IStep
  {
    bool CapProcess();
    bool IsAsync();    
    Task ProcessAsync();
    void Process();
    IServiceProvider Services { get; set; }
    string Name { get; set; }
    int TimeTaken { get; set; }    
    int ProcessedItemsCount { get; set; }    
  }
}