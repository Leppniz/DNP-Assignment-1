using System.Collections.Generic;
using FileData;
using Models;

namespace Assignment1.Data
{
    public class Adapter
    {
        public Adapter()
        {
        }
        public void AddAdult(Adult adult)
        {
            FileContext fileContext = new FileContext();
            fileContext.Adults.Add(adult);
        }
        

        public List<Adult> getAdults()
        {
            FileContext fileContext = new FileContext();
            return (List<Adult>) fileContext.Adults;
        }
        public List<Family> getFamilies()
        {
            FileContext fileContext = new FileContext();
            return (List<Family>) fileContext.Families;
        }
        public List<Child> getChildren()
        {
            FileContext fileContext = new FileContext();
            return (List<Child>) fileContext.Children;
        }
        
    }
}