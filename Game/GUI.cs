﻿using SFML.Graphics;
using SFML.System;
using SFML.Window;

using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class GUI
    {
        public class Component
        {
            public enum STATE { IDLE, HOVER, ACTIVE };
            public delegate void Function();
            public Function onClick;
            public Function onMouseOver;

            public STATE state;
            public bool oldButtonState;
            public SFML.Graphics.Text text;
            public RectangleShape shape;
            public Color textColor, shapeColor;

            public Component() { }

            public Component(uint characterSize, string text, Font font, Color textColor, Color shapeColor)
            {
                state = STATE.IDLE;
                oldButtonState = Mouse.IsButtonPressed(Mouse.Button.Left);
                this.textColor = textColor;
                this.shapeColor = shapeColor;
                this.text = new SFML.Graphics.Text(text, font);
            }

            public virtual void Update(ref RenderWindow window) { }
            public virtual void Render(ref RenderWindow window) { }
        }

        public class Button : Component
        {
            bool gainSound;
            public Color
                textIdleColor,
                textHoverColor,
                textActiveColor;

            public Button(
                Vector2f shapeSize,
                Vector2f centrePos,
                uint characterSize,
                string text,
                Font font,
                Color idle,
                Color hover,
                Color active
            ) : base(characterSize, text, font, idle, new Color(Color.Transparent))
            {
                gainSound = false;
                textIdleColor = idle;
                textHoverColor = hover;
                textActiveColor = active;
                shape = new RectangleShape(shapeSize)
                {
                    FillColor = new Color(0, 0, 0, 0),
                    Origin = new Vector2f(shapeSize.X / 2f, shapeSize.Y / 2f),
                    Position = new Vector2f(centrePos.X, centrePos.Y)
                };

                this.text = new SFML.Graphics.Text(text, font)
                {
                    CharacterSize = characterSize,
                    Position = new Vector2f(centrePos.X, centrePos.Y),
                    FillColor = new Color(idle),
                    OutlineColor = new Color(Color.Black),
                    OutlineThickness = 1f
                };

                Vector2f origin = new Vector2f(0f, 0f)
                {
                    X = this.text.GetGlobalBounds().Width / 2f,
                    Y = shapeSize.Y / 2f
                };
                this.text.Origin = new Vector2f(origin.X, origin.Y);
            }

            public override void Update(ref RenderWindow window)
            {
                Vector2i mousePos = Mouse.GetPosition(window);
                if (shape.GetGlobalBounds().Contains(mousePos.X, mousePos.Y))
                {
                    text.FillColor = new Color(textHoverColor);
                    onMouseOver?.Invoke();
                    if (!gainSound)
                    {
                        Program.resources.sounds["btn_hover"].Play();
                        gainSound = true;
                    }

                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                    {
                        Program.resources.sounds["btn_click"].Play();
                        text.FillColor = new Color(textActiveColor);
                        onClick?.Invoke();
                    }
                }
                else
                {
                    text.FillColor = new Color(textIdleColor);
                    gainSound = false;
                }
            }

            public override void Render(ref RenderWindow window)
            {
                //shape.FillColor = new Color(Color.Red);
                window.Draw(shape);
                window.Draw(text);
            }
        }

        public class Text : Component
        {
            public Text(
                Vector2f centrePos,
                uint characterSize,
                string text,
                Font font,
                Color color
            ) : base(characterSize, text, font, color, new Color(Color.Transparent))
            {
                this.text = new SFML.Graphics.Text(text, font)
                {
                    CharacterSize = characterSize,
                    Position = new Vector2f(centrePos.X, centrePos.Y),
                    FillColor = new Color(color),
                    OutlineColor = new Color(Color.Black),
                    OutlineThickness = 1f,
                };

                this.shape = new RectangleShape()
                {
                    Size = new Vector2f(this.text.GetGlobalBounds().Width, this.text.GetGlobalBounds().Height),
                    Position = new Vector2f(centrePos.X, centrePos.Y),
                    FillColor = new Color(Color.Transparent),
                    Origin = new Vector2f(
                        this.text.GetGlobalBounds().Width / 2f,
                        this.text.GetGlobalBounds().Height / 2f
                    )
                };

                this.text.Origin = new Vector2f(
                    (this.text.GetGlobalBounds().Width + characterSize / 8f) / 2f,
                    (this.text.GetGlobalBounds().Height + characterSize / 2f) / 2f
                );
            }


            public override void Update(ref RenderWindow window) { }

            public override void Render(ref RenderWindow window)
            {
                //shape.FillColor = new Color(Color.Red);
                window.Draw(shape);
                window.Draw(text);
            }
        }

        public class Texture : Component
        {
            bool gainSound;
            RectangleShape textureShape;
            Color OutlineIdle, OutlineHover, OutlineActive;

            public Texture(
                int id,
                List<Entity> carCollection,
                Vector2f shapeSize,
                Vector2f textureSize,
                Vector2f centrePos,
                Color OutlineIdle,
                Color OutlineHover,
                Color OutlineActive,
                float rotation = 0f
            )
            {
                gainSound = false;
                oldButtonState = Mouse.IsButtonPressed(Mouse.Button.Left);
                this.OutlineIdle = OutlineIdle;
                this.OutlineHover = OutlineHover;
                this.OutlineActive = OutlineActive;
                shape = new RectangleShape(shapeSize)
                {
                    FillColor = new Color(0, 0, 0, 0),
                    Origin = new Vector2f(shapeSize.X / 2f, shapeSize.Y / 2f),
                    Position = new Vector2f(centrePos.X, centrePos.Y),
                    OutlineThickness = 10f,
                    OutlineColor = new Color(OutlineIdle),
                    Rotation = rotation,
                };
                Sprite car = carCollection[id].sprite;
                textureShape = new RectangleShape(textureSize)
                {
                    Origin = new Vector2f(textureSize.X / 2f, textureSize.Y / 2f),
                    Position = new Vector2f(centrePos.X, centrePos.Y),
                    Texture = new SFML.Graphics.Texture(car.Texture),
                    TextureRect = new IntRect(
                        car.TextureRect.Left,
                        car.TextureRect.Top,
                        car.TextureRect.Width,
                        car.TextureRect.Height
                    ),
                    Rotation = rotation,
                };
            }

            public Texture(
                string filename,
                Vector2f textureSize,
                Vector2f centrePos,
                float rotation = 0f
            )
            {
                gainSound = false;
                shape = new RectangleShape();
                textureShape = new RectangleShape(textureSize)
                {
                    Texture = new SFML.Graphics.Texture(filename),
                    Origin = new Vector2f(textureSize.X / 2f, textureSize.Y / 2f),
                    Position = new Vector2f(centrePos.X, centrePos.Y),
                    Rotation = rotation,
                };
            }

            public override void Update(ref RenderWindow window)
            {
                Vector2i mousePos = Mouse.GetPosition(window);
                if (shape.GetGlobalBounds().Contains(mousePos.X, mousePos.Y))
                {
                    shape.OutlineColor = new Color(OutlineHover);
                    onMouseOver?.Invoke();

                    if (!gainSound)
                    {
                        Program.resources.sounds["btn_hover"].Play();
                        gainSound = true;
                    }

                    bool buttonState = Mouse.IsButtonPressed(Mouse.Button.Left);

                    if (buttonState && !oldButtonState)
                    {
                        Program.resources.sounds["btn_click"].Play();
                        shape.OutlineColor = new Color(OutlineActive);
                        onClick?.Invoke();
                    }

                    oldButtonState = buttonState;
                }
                else
                {
                    shape.OutlineColor = new Color(OutlineIdle);
                    gainSound = false;
                }
            }

            public override void Render(ref RenderWindow window)
            {
                window.Draw(shape);
                window.Draw(textureShape);
            }
        }

        public class Input : Component
        {
            public uint maxLen;
            public Dictionary<Keyboard.Key, string> keys;
            public Dictionary<Keyboard.Key, bool> keyStates;
            public Function onEnter;

            public Input(
                Vector2f position,
                uint characterSize, 
                Font font, 
                Color textColor, 
                Dictionary<Keyboard.Key, string> keys,
                uint maxLen = 21
            ) : base(characterSize, null, font, textColor, new Color(Color.Transparent))
            {
                this.maxLen = maxLen;
                keyStates = new Dictionary<Keyboard.Key, bool>();
                this.keys = keys;
                foreach (var key in keys.Keys.ToList())
                    keyStates[key] = true;

                text = new SFML.Graphics.Text(null, font)
                {
                    CharacterSize = characterSize,
                    Position = new Vector2f(position.X, position.Y),
                    FillColor = new Color(textColor),
                    OutlineColor = new Color(Color.Black),
                    OutlineThickness = 1f,
                };
            }

            public override void Update(ref RenderWindow window)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Return))
                    onEnter?.Invoke();

                var keyList = keys.Keys.ToList();

                foreach (var key in keyList)
                {
                    string value = keys[key];

                    bool currKeyState = Keyboard.IsKeyPressed(key);
                    bool oldKeyState = keyStates[key];

                    if (currKeyState && !oldKeyState)
                    {
                        string str = text.DisplayedString;

                        if (value == "BackSpace")
                        {
                            text.DisplayedString = str.Remove(str.Length - 1); ;
                        }
                        else
                        {
                            if (!Keyboard.IsKeyPressed(Keyboard.Key.LShift) &&
                                !Keyboard.IsKeyPressed(Keyboard.Key.RShift))
                                value = value.ToLower();

                            if (str.Length < maxLen)
                                text.DisplayedString = str + value;
                        }
                    }                    
                    keyStates[key] = currKeyState;
                }
            }

            public override void Render(ref RenderWindow window)
            {
                window.Draw(text);
            }
        }

        public class List : Component
        {
            List<ListItems> items;

            public List()
            {
                items = new List<ListItems>();
            }

            public void Add(ListItems item)
            {
                items.Add(item);
            }

            public override void Update(ref RenderWindow window)
            {
                foreach (var item in items)
                    item.Update(ref window);
            }

            public override void Render(ref RenderWindow window)
            {
                foreach (var item in items)
                    item.Render(ref window);
            }
        }

        public class ListItems : Component
        {
            List<Component> components;

            public ListItems()
            {
                components = new List<Component>();
            }

            public void Add(Component c)
            {
                components.Add(c);
            }

            public void Clear()
            {
                components.Clear();
            }

            public override void Update(ref RenderWindow window)
            {
                foreach (var item in components)
                    item.Update(ref window);
            }

            public override void Render(ref RenderWindow window)
            {
                foreach (var item in components)
                    item.Render(ref window);
            }
        }
    }
}
