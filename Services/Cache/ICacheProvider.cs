﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Cache
{
    public interface ICacheProvider
    {
        object Get(string key);
        void Set(string key, object data, int cacheTime);
        bool IsSet(string key);
        void Invalidate(string key);
    }
}
