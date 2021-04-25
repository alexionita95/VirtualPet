using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace VirtualPet.Core
{
    public interface IPetInteractor
    {
        PetInfo GetPetInfo();
        Point GetPetLocation();
        Rectangle GetPetBounds();
        void SetPetLocation(Point p);

    }
}
