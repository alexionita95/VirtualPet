﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Core
{
    public interface IPlugin
    {
        public void SetInteractor(IPetInteractor interactor);
        public void Initialize();
    }
}
