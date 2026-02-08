using System;
using System.Collections;
using System.Linq.Expressions;

namespace BullBrukBruker
{
    public interface IUserSpecificDataAccessor
    {
        public IEnumerator LoadData();
        public IEnumerator SaveData();
    }
}