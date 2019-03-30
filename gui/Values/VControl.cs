using System.Windows.Forms;

namespace Lumen.Lang.Libraries.Visual {
    public interface VControl: Value {
        Control Control { get; }
    }
}
