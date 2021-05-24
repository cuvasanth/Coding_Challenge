using System.Collections.Generic;
using System.Threading.Tasks;
using VertMarket_programming_Challenge.Model;

namespace VertMarket_programming_Challenge.Interfaces
{
   
    public interface IMagazineStoreSa
    {
       
        Task<List<string>> GetCategories();
       
        Task<List<Subscriber>> GetSubscribers();
        
        Task<List<Magazine>> GetMagazines(string category);
        
        Task<SubmissionResponse> SubmitAnswer(IEnumerable<string> subcribers);

        Task<SubmissionResponse> IdentifySubcribersToAllCategories();
    }
}
