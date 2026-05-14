using UnityEngine;

namespace ShahvaizJ.CustomLog
{
    public class LogDemo : MonoBehaviour
    {
        public void DoPrint()   => Log.Print("Print");
        public void DoWarning() => Log.Warning("Warning");
        public void DoError()   => Log.Error("Error");
        public void DoBlue()    => Log.Blue("Blue");
        public void DoRed()     => Log.Red("Red");
        public void DoYellow()  => Log.Yellow("Yellow");
        public void DoGreen()   => Log.Green("Green");
        public void DoCyan()    => Log.Cyan("Cyan");
        public void DoOrange()  => Log.Orange("Orange");
    }
}
