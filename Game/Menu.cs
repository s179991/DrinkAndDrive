using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.Xml;

namespace Game
{
    /// <summary>
    /// Klasa tworząca obraz menu.
    /// </summary>
    class Menu
    {
        /// <summary>
        /// Sktruktura pomocnicza do wyświetlania wyników graczy.
        /// </summary>
        private struct Player
        {
            /// <summary>Zmienna przechowująca nazwę gracza.</summary>
            public string name;
            /// <summary>Zmienna przechowująca wynik gracza.</summary>
            public int score;
            /// <summary>Zmienna przechowująca czas gry gracza.</summary>
            public float time;

            /// <summary>
            /// Konstruktor - inicjalizacja zmiennych.
            /// </summary>
            /// <param name="name">Nazwa gracza.</param>
            /// <param name="score">Wynik gracza.</param>
            /// <param name="time">Całkowity czas gry.</param>
            public Player(string name, int score, float time)
            {
                this.name = name;
                this.score = score;
                this.time = time;
            }
        }

        /// <summary>Referencja do obiektu zasobów gry.</summary>
        Resources resources;
        /// <summary>Referencja do silnika gry.</summary>
        Engine engine;

        /// <summary>Zmienna określająca kolor tekstu buttona w stanie bezczynności.</summary>
        Color idle = new Color(0xE0, 0xFF, 0xFF);
        /// <summary>Zmienna określająca kolor tekstu buttona w stanie najechania kursorem.</summary>
        Color hover = new Color(0xF5FFFA);
        /// <summary>Zmienna określająca kolor tekstu buttona w stanie aktywnym.</summary>
        Color active = new Color(0xFF, 0xFF, 0xFF);

        /// <summary>Zmienna przechowująca wyświetlane komponenty GUI na ekranie.</summary>
        List<GUI.Component> components;

        /// <summary>
        /// Konstruktor - inicjalizacja zmiennych referencyjnych.
        /// </summary>
        /// <param name="engine">Referencja do obiektu silnika gry.</param>
        /// <param name="resources">Referencja do obiektu zasobów gry.</param>
        public Menu(Engine engine, Resources resources)
        {
            // zapisanie referencji do silnika gry
            this.engine = engine;
            // zapisanie referencji do zasobów
            this.resources = resources;
            // utworzenie obiektów listy komponentów w pamięci
            components = new List<GUI.Component>();
            // pierwsze uruchomienie - menu główne
            InitMainMenu();
        }

        /// <summary>
        /// Główna metoda aktualizująca wyświetlane obiekty w stanie pauzy.
        /// </summary>
        /// <param name="window">Referencja do obiektu głównego okna aplikacji.</param>
        public void Update(ref RenderWindow window)
        {
            // próba aktualizacja każdego komponentu
            // zabezpieczenie przed modyfikacją listy komponentów
            // z poziomu akcji Update 
            try
            {
                foreach (var component in components)
                    component.Update(ref window);
            }
            catch (Exception) 
            {
                // taki wyjątek nie jest obsługiwany
                // utworzeniem nowych komponentów zajmują się inne funkcje klasy Menu
            }
        }

        /// <summary>
        /// Główna metoda renderująca obiekty GUI na wskazanym oknie.
        /// </summary>
        /// <param name="window">Referencja do obiektu głównego okna aplikacji.</param>
        public void Render(ref RenderWindow window)
        {
            // rendering każdego komponentu z listy
            foreach (var component in components)
                component.Render(ref window);
        }

        /// <summary>
        /// Metoda inicjalizująca komponenty GUI podczas wyświetlania głównego menu.
        /// </summary>
        public void InitMainMenu()
        {
            // czyszczenie listy komponentów
            components.Clear();
            // menu główne - wszystkie komponenty ustawione na środku (taka sama pozycja na osi X dla każdego)
            Vector2f size = new Vector2f(1000f, 100f);
            float posX = resources.options.winWidth / 2f;
            // utworzenie przycisków menu głównego
            GUI.Button ContinueBtn   = new GUI.Button(size, new Vector2f(posX, 100f), 78, "continue", resources.font, idle, hover, active);
            GUI.Button StartAgainBtn = new GUI.Button(size, new Vector2f(posX, 206f), 78, "start again", resources.font, idle, hover, active);
            GUI.Button SelectCarBtn  = new GUI.Button(size, new Vector2f(posX, 312f), 78, "select car", resources.font, idle, hover, active);
            GUI.Button ResultsBtn    = new GUI.Button(size, new Vector2f(posX, 418f), 78, "other players results", resources.font, idle, hover, active);
            GUI.Button ControlBnt    = new GUI.Button(size, new Vector2f(posX, 524f), 78, "show control", resources.font, idle, hover, active);
            GUI.Button ExitBtn       = new GUI.Button(size, new Vector2f(posX, 630f), 78, "exit game", resources.font, idle, hover, active);
            // dodanie delegaty do obsługi zdarzenia kliknięcia
            ContinueBtn.onClick   = new GUI.Component.Function(engine.OnContinue);
            StartAgainBtn.onClick = new GUI.Component.Function(engine.OnStartAgain);
            SelectCarBtn.onClick  = new GUI.Component.Function(InitSelectCars);
            ResultsBtn.onClick    = new GUI.Component.Function(InitPlayersResults);
            ControlBnt.onClick    = new GUI.Component.Function(InitControl);
            ExitBtn.onClick       = new GUI.Component.Function(engine.OnExit);
            // dodanie komponentów do listy
            components.Add(ContinueBtn);
            components.Add(StartAgainBtn);
            components.Add(SelectCarBtn);
            components.Add(ResultsBtn);
            components.Add(ControlBnt);
            components.Add(ExitBtn);
        }

        /// <summary>
        /// Metoda inicjalizująca komponenty GUI podczas wyświeltania obsługiwanych klawiszy.
        /// </summary>
        private void InitControl()
        {
            // czyszczenie listy komponentów
            components.Clear();
            // wyświetlany tekst z obsługiwanymi klawiszami
            string control = "Control: \n" +
                " ^ / w - move top \n" +
                " < / a - move left \n" +
                " v / s - move bottom \n" +
                " > / d - move right \n" +
                " P / Esc - open / close menu \n" +
                " B - show / hide hitboxes";
            // utworzenie komponentu tekstu
            GUI.Text ctrl = new GUI.Text(new Vector2f(512f, 384f), 60, control, resources.font, hover); 
            // utworzenie powrotu do menu głównego
            GUI.Button back = new GUI.Button(new Vector2f(200f, 80f), new Vector2f(900f, 720f), 70, "back", resources.font, idle, hover, active)
            {
                // obsługa kliknięcia w przycisk back powoduje przejście do menu głównego
                onClick = new GUI.Component.Function(InitMainMenu)
            };
            // dodanie komponentów do listy
            components.Add(ctrl);
            components.Add(back);
        }

        /// <summary>
        /// Metoda inicjalizująca komponenty GUI podczas wyświetlania wyników graczy.
        /// </summary>
        private void InitPlayersResults()
        {
            // inicjalizacja wyświetlanych wyników graczy zaczyna się od strony 1
            int page = 1;
            // wywołanie metody ładującej daną stronę wyników jako komponenty
            ShowPlayersResults(page);
        }

        /// <summary>
        /// Metoda inicjalizująca komponenty GUI podczas wyświetlania opcji wyboru pojazdu.
        /// </summary>
        private void InitSelectCars()
        {
            // czyszczenie listy
            components.Clear();
            // utworzenie listy z wielkością kształtu kolejnych pojazdów
            List<Vector2f> shapeSizes = new List<Vector2f>
            {
                new Vector2f(130f, 200f), new Vector2f(130f, 200f),
                new Vector2f(130f, 200f), new Vector2f(130f, 200f),
                new Vector2f(130f, 200f), new Vector2f(130f, 200f),
                new Vector2f(130f, 200f), new Vector2f(130f, 230f),
                new Vector2f(115f, 300f), new Vector2f(120f, 250f),
                new Vector2f(140f, 380f), new Vector2f(140f, 350f),
                new Vector2f(140f, 390f), new Vector2f(130f, 370f),
            };
            // utworzenie listy z wielkościami tekstur kolejnych pojazdów
            List<Vector2f> textureSize = new List<Vector2f>
            {
                new Vector2f(100f, 190f), new Vector2f(100f, 190f),
                new Vector2f(100f, 190f), new Vector2f(100f, 190f),
                new Vector2f(100f, 190f), new Vector2f(100f, 190f),
                new Vector2f(100f, 190f), new Vector2f(100f, 220f),
                new Vector2f(105f, 290f), new Vector2f(110f, 240f),
                new Vector2f(130f, 370f), new Vector2f(130f, 340f),
                new Vector2f(130f, 380f), new Vector2f(120f, 360f),
            };
            // utworzenie listy z pozycją kolejnych pojazdów
            List<Vector2f> pos = new List<Vector2f>
            {
                new Vector2f(80f, 150f), new Vector2f(253f, 150f),
                new Vector2f(426f, 150f),new Vector2f(599f, 150f),
                new Vector2f(772f, 150f),new Vector2f(945f, 150f),
                new Vector2f(80f, 375f), new Vector2f(80f, 620f),
                new Vector2f(339f, 702f),new Vector2f(253f, 400f),
                new Vector2f(772f, 465f),new Vector2f(426f, 448f),
                new Vector2f(945f, 475f),new Vector2f(599f, 465f),
            };
            // pętla ładująca pojazdy jako komponenty
            for (int id = 0; id < 14; id++)
            {
                // pobranie identyfikatora pojazdu
                int carID = id;
                // zrotowany jest tylko pojazd do ID = 8
                float rotation = (id == 8) ? 90f : 0f;
                // utworzenie komponentu GUI
                components.Add(new GUI.Texture(id, resources.carCollection, shapeSizes[id], textureSize[id], pos[id], new Color(Color.Transparent), new Color(Color.Green), new Color(Color.Blue), rotation)
                    {
                        // obsługa zdarzenia kliknięcia
                        onClick = new GUI.Component.Function(delegate() { engine.OnSelectCar(carID); })
                    }
                );
            }
            // dodanie przycisku powrotu
            components.Add(new GUI.Button(new Vector2f(200f, 80f), new Vector2f(900f, 720f), 70, "back", resources.font, idle, hover, active)
                {
                    onClick = new GUI.Component.Function(InitMainMenu)
                }
            );
        }

        /// <summary>
        /// Metoda inicjalizująca komponenty GUI podczas wyświetlania końca gry.
        /// </summary>
        public void InitGameResult()
        {
            // czyszczenie listy komponentów 
            components.Clear();
            // odtworzenie odpowiednich dźwieków
            resources.sounds["menu_music"].Play();
            resources.sounds["traffic_noise"].Stop();
            resources.sounds["bg_sound"].Stop();
            // pobranie wyników gracza
            int score = engine.score;
            double gameTime = engine.gameTime.GetCurrentTime();
            double alcoTime = engine.alcoTime.GetCurrentTime();
            // utworzenie listy wszystkich graczy
            CreatePlayerList(out List<Player> playerList);
            int place = playerList.Count + 1;
            // szukanie miejsca gracza
            for (int i = 0; i < playerList.Count; i++)
            {
                if (score > playerList[i].score)
                {
                    place = i + 1;
                    break;
                }
            }
            // dodanie elementów stacyjnych
            components.Add(new GUI.Text(new Vector2f(512f, 150f), 90, "YOU LOSE!", resources.font, hover));
            components.Add(new GUI.Text(new Vector2f(200f, 300f), 60, "User name:", resources.font, hover));
            components.Add(new GUI.Text(new Vector2f(512f, 400f), 50, $"Your place: {place}", resources.font, hover));
            components.Add(new GUI.Text(new Vector2f(512f, 500f), 50, $"Your score {score}, game time: {(int)gameTime} sec", resources.font, hover));
            components.Add(new GUI.Text(new Vector2f(512f, 580f), 50, $"drinking time: {(int)alcoTime} sec", resources.font, hover));
            // jeśli gracz uzyskał pierwsze miejsce pojawia się napis NEW HIGH SCORE w prawym górnym rogu
            if (place == 1)
            {
                components.Add(new GUI.Texture(
                        "..\\..\\resource\\images\\high_score.png", 
                        new Vector2f(250f, 160f), 
                        new Vector2f(860f, 130f), 
                        0f
                    )
                );
            }
            // utworzenie komponentu wpisywania tekstu w celu pobrania nazwy gracza do zapisania w dok XML
            GUI.Input input = new GUI.Input(new Vector2f(370f, 260f), 60, resources.font, idle, resources.keys);
            // utworzenie obsługi potwierdzenia enterem
            input.onEnter = new GUI.Component.Function(delegate () { Save(input.text, score, gameTime); });
            // dodanie komponentu
            components.Add(input);
            // dodanie osobnego przycisku zapisu
            components.Add(new GUI.Button(new Vector2f(700f, 95f), new Vector2f(512f, 720f), 70, "save and start again", resources.font, idle, hover, active)
                {
                    onClick = new GUI.Component.Function(delegate() { Save(input.text, score, gameTime); })
                }
            );
        }

        /// <summary>
        /// Metoda pokazująca wyniki graczy na ekranie względem wskazanej strony.
        /// </summary>
        /// <param name="page">Strona wyświetlanych wyników graczy.</param>
        private void ShowPlayersResults(int page)
        {
            // ilość graczy na stronie
            int itemsPerPage = 6;
            // czyszczenie listy componentów
            components.Clear();
            // tworzenie listy z zapisanymi graczami
            CreatePlayerList(out List<Player> playerList);
            // paginacja
            CreatePagination(playerList, itemsPerPage, page);
            // dodawanie elementów stacyjnych
            LoadStationElements(page);
            // dodanie elementów graczy
            LoadPlayersResults(playerList, page, itemsPerPage);
        }

        /// <summary>
        /// Metoda tworząca pełną listę wyników wszystkich graczy.
        /// </summary>
        /// <param name="list">Lista wyników graczy.</param>
        private void CreatePlayerList(out List<Player> list)
        {
            list = new List<Player>();
            XmlNodeList playerNodes = resources.document.GetElementsByTagName("player");
            // utworzenie wewnętrznej listy z graczami
            foreach (XmlNode node in playerNodes)
            {
                string name = node.Attributes["name"].Value;
                int score = Convert.ToInt32(node.Attributes["score"].Value);
                float time = float.Parse(node.Attributes["time"].Value);
                list.Add(new Player(name, score, time));
            }
            // sortowanie listy
            list.Sort(
                delegate (Player A, Player B)
                {
                    if (A.score > B.score) return -1;
                    else if (A.score < B.score) return 1;
                    return 0;
                }
            );
        }

        /// <summary>
        /// Metoda wyznaczająca ilość stron oraz tworzenie ich komponentów.
        /// </summary>
        /// <param name="list">Lista wyników wszystkich graczy.</param>
        /// <param name="itemsPerPage">Ilość obiektów na stronie.</param>
        /// <param name="page">Ilość obiektów na stronie.</param>
        private void CreatePagination(List<Player> list, int itemsPerPage, int page)
        {
            // obliczenie ilości stron
            int pages = (int)Math.Ceiling((double)list.Count / (double)itemsPerPage);
            // dodanie elementów stron do listy komponentów
            for (int i = 1; i <= pages; i++)
            {
                int pagePtr = i;
                GUI.Button button = new GUI.Button(new Vector2f(80f, 50f), new Vector2f(200f + i * 80f, 100f), 40, $"{i}", resources.font, idle, hover, active);
                if (page != pagePtr)
                    button.onClick = new GUI.Component.Function(delegate () { ShowPlayersResults(pagePtr); });
                
                
                components.Add(button);
            }
        }

        /// <summary>
        /// Metoda ładująca komponenty stacyjne (napisy i przycisk powrotu do menu głównego) względem aktualnej strony.
        /// </summary>
        /// <param name="page">Strona wyświetlanych wyników graczy.</param>
        private void LoadStationElements(int page)
        {
            // dodanie elementów stacyjnych
            components.Add(new GUI.Text(new Vector2f(60f, 150f), 40, "Id:", resources.font, hover));
            components.Add(new GUI.Text(new Vector2f(250f, 150f), 40, "Name:", resources.font, hover));
            components.Add(new GUI.Text(new Vector2f(500f, 150f), 40, "Score:", resources.font, hover));
            components.Add(new GUI.Text(new Vector2f(830f, 150f), 40, "Total time [sec]:", resources.font, hover));
            components.Add(new GUI.Text(new Vector2f(140f, 100f), 40, $"Pages ({page}):", resources.font, hover));

            components.Add(new GUI.Button(new Vector2f(200f, 80f), new Vector2f(900f, 720f), 70, "back", resources.font, idle, hover, active)
                {
                    onClick = new GUI.Component.Function(InitMainMenu)
                }
            );
        }

        /// <summary>
        /// Metoda ładująca komponenty GUI wyników graczy względem wskazanej strony.
        /// </summary>
        /// <param name="list">Lista wyników wszystkich graczy.</param>
        /// <param name="page">Strona wyświetlanych wyników graczy.</param>
        /// <param name="itemsPerPage">Ilość obiektów na stronie.</param>
        private void LoadPlayersResults(List<Player> list, int page, int itemsPerPage)
        {
            // określenie maksymalnej długości tekstu wyświetlanego w miejscu nazwy gracza
            int maxNameLen = 10;
            // dodanie elementów graczy
            for (int i = 0; i < itemsPerPage; i++)
            {
                // wyznaczenie identyfikatora l.p. zależnego od aktualnej strony
                int id = i + itemsPerPage * (page - 1);
                // pobranie nazwy i sprawdzenie warunku maksymalnej długości tekstu
                string name = list[id].name.Trim();
                if (name.Length > maxNameLen)
                {
                    // tekst jest zbyt długi
                    string newName = "";
                    // wyznaczenie nowego stringu - kopia pierwszych znaków
                    for (int j = 0; j < maxNameLen - 3; j++)
                        newName += name[j];
                    // oraz dodanie ... na końcu wyświetlanego tekstu
                    name = newName.Trim() + "...";
                }
                // dodanie komponentów do listy
                components.Add(new GUI.Text(new Vector2f(250f, 230f + i * 80f), 40, $"{name}", resources.font, idle));
                components.Add(new GUI.Text(new Vector2f(500f, 230f + i * 80f), 40, $"{list[id].score}", resources.font, idle));
                components.Add(new GUI.Text(new Vector2f(830f, 230f + i * 80f), 40, $"{list[id].time}", resources.font, idle));
                components.Add(new GUI.Text(new Vector2f(60f, 230f + i * 80f), 40, $"{id + 1}.", resources.font, idle));
            }
        }

        /// <summary>
        /// Metoda zapisująca wynik gracza w dokumencie XML.
        /// </summary>
        /// <param name="text">Wskazana nazwa gracza.</param>
        /// <param name="score">Wynik gracza.</param>
        /// <param name="time">Całkowity czas gry.</param>
        private void Save(Text text, int score, double time)
        {
            // walidacja wprowadzonego tekstu
            if (text.DisplayedString.Trim().Length > 0)
            {
                // pobranie globalnego czasu gry wszystkich graczy
                XmlAttribute totalTimeAttr = resources.document.GetElementsByTagName("game")[0].Attributes["total_time"];
                int totalTime = Convert.ToInt32(totalTimeAttr.Value);
                // utworzenie węzła graczy
                XmlNode playersNode = resources.document.GetElementsByTagName("players")[0];
                // utworzenie nowego elementu XML
                XmlElement newPlayer = resources.document.CreateElement("player");
                // utworzenie atrybutów gracza
                newPlayer.SetAttribute("name", text.DisplayedString);
                newPlayer.SetAttribute("score", score.ToString());
                newPlayer.SetAttribute("time", ((int)time).ToString());
                // zwiększenie globalnego czasu gry
                totalTime += (int)time;
                // zapis globalnego czasu do XML
                totalTimeAttr.InnerText = totalTime.ToString();
                // dodanie nowego gracza do XML
                playersNode.AppendChild(newPlayer);
                // zapis dokument XML do tego samego pliku
                resources.document.Save(C.XML_DOCUMENT_PATH);
                // ponowny start gry
                engine.OnStartAgain();
            }
        }
    }
}
