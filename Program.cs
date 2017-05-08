using System;
using System.IO;

namespace FixToLuaNamespaces {
    class Program {
        static void Main(string[] args) {
            // Fix file. (Very naive, but works for my personal purposes.)
            string[] buffer = File.ReadAllLines(args[0]);
            using (StreamReader stream = new StreamReader(File.Open(args[0], FileMode.Open))) {
                int lineID = 0;
                string line;
                while ((line = stream.ReadLine()) != null) {
                    if (line.Split('"').Length - 1 > 1 && line.Contains("::")) { // At least 2 occurances of " and one occurance of ::.
                        // Find left and right quotation.
                        int _leftQuote = -1;
                        int _rightQuote = -1;
                        int _nsIndex = line.IndexOf("::");
                        for (int i = _nsIndex; i >= 0; --i) {
                            if (line[i] == '"') {
                                _leftQuote = i;
                                break;
                            }
                        }
                        for (int i = _nsIndex; i < line.Length; ++i) {
                            if (line[i] == '"') {
                                _rightQuote = i;
                                break;
                            }
                        }

                        // Ensure quotes encase a ToLua++ type name.
                        string _variable = line.Substring(_leftQuote, _rightQuote - _leftQuote);
                        if (_variable.Split(new string[] { "::" }, StringSplitOptions.None).Length - 1 == 1) {
                            _nsIndex = _variable.IndexOf("::");
                            buffer[lineID] = line.Substring(0, _leftQuote + 1) + _variable.Substring(_nsIndex + 2, _variable.Length - (_nsIndex + 2)) + line.Substring(_rightQuote, line.Length - _rightQuote);
                        }
                    }
                    ++lineID; // Step line ID.
                }
            }
            File.WriteAllLines(args[0], buffer); // Actually overwrite file lines.
        }
    }
}
