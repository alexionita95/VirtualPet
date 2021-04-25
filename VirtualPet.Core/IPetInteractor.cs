using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace VirtualPet.Core
{
    public interface IPetInteractor
    {
        PetInfo GetPetInfo();
        Point GetPetLocation();
        Point GetScreenPetLocation();
        Rectangle GetPetBounds();
        void SetPetLocation(Point p);
        void SpawnToy(Toy t);
        void SpawnDialog(UserControl dialog);

    }
}
