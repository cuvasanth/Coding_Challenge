using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertMarket_programming_Challenge.Model
{
    public class Magazine
    {
            public int Id { get; set; }
            
            public string Name { get; set; }
            
            public string Category { get; set; }
    }

    public class MagazineResponse : ApiResponse
    {
       public List<Magazine> Data { get; set; }
    }

    public class CategoriesResponse : ApiResponse
    {
       public List<string> Data { get; set; }
    }
    public class Submission
    {
       
        public string TotalTime { get; set; }
      
        public bool AnswerCorrect { get; set; }
        
        public List<string> ShouldBe { get; set; }
    }

    public class SubmissionResponse : ApiResponse
    {
        public Submission Data { get; set; }
    }

    public class Subscriber
    {
        
        public string Id { get; set; }
        
        public string FirstName { get; set; }
       
        public string LastName { get; set; }
       
        public List<int> MagazineIds { get; set; } = new List<int>();
    }

    public class SubscriberResponse : ApiResponse
    {
        public List<Subscriber> Data { get; set; }
    }

    public class ApiAnswer
    {
       public IEnumerable<string> Subscribers { get; set; }
    }
}
