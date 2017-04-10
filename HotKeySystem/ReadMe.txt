Dies ist ein kleines HotKey-System was ich für command-based WPF-Anwendungen geschrieben habe.
Die Grundidee ist es hotkeys über die app.config dynamisch änderbar zu machen, um sie nicht hard-zu-coden.
Ein Hotkey besteht immer aus einer Gesture z.B. Key- oder MouseGesture und einem zugehörigen Command, welcher durch das Tastenkürzel aktiviert wird. 
Optional können Hotkeys auch noch eine Beschreibung tragen, was ganz hilfreich ist, um sie lokalisiert und z.B. in einem Hilfe-Fenster anzuzeigen.
Ein Command fordert beim HotKeyProvider einen hotkey an. Dieser überprüft, ob der angeforderte hotkey schon existiert und gibt ihn dann zurück.
Sollte der hotkey nicht existiert, dann erstellt der HotKeyProvider diesen und speichert ihn ab.
Durch das System kann man schnell neue Hotkeys erstellen und hat sie dynamisch änderbar und gebündelt in der app.config bzw. in der .xaml des jeweiligen Fensters stehen.

PS: F1 fürs Hilfe-Fenster drücken ;)