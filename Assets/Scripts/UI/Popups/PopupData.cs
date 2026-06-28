using System;
using System.Collections.Generic;

namespace OctoGames.UI.Popups
{
    public readonly struct PopupButtonData
    {
        public readonly string Label;
        public readonly Action OnClick;

        public PopupButtonData(string label, Action onClick)
        {
            Label = label;
            OnClick = onClick;
        }
    }

    /// <summary>Immutable popup content: title, body, 1–5 buttons with callbacks.</summary>
    public sealed class PopupData
    {
        public const int MinButtons = 1;
        public const int MaxButtons = 5;

        public string Title { get; }
        public string Body { get; }
        public IReadOnlyList<PopupButtonData> Buttons { get; }
        public bool CloseOnButtonClick { get; }

        internal PopupData(string title, string body, IReadOnlyList<PopupButtonData> buttons, bool closeOnButtonClick)
        {
            Title = title;
            Body = body;
            Buttons = buttons;
            CloseOnButtonClick = closeOnButtonClick;
        }
    }

    /// <summary>Fluent builder; enforces the 1–5 button rule in one place.</summary>
    public sealed class PopupBuilder
    {
        private readonly List<PopupButtonData> _buttons = new();
        private string _title = string.Empty;
        private string _body = string.Empty;
        private bool _closeOnButtonClick = true;

        public static PopupBuilder Create() => new PopupBuilder();

        public PopupBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public PopupBuilder Body(string body)
        {
            _body = body;
            return this;
        }

        public PopupBuilder Button(string label, Action onClick)
        {
            _buttons.Add(new PopupButtonData(label, onClick));
            return this;
        }

        public PopupBuilder CloseOnButtonClick(bool value)
        {
            _closeOnButtonClick = value;
            return this;
        }

        public PopupData Build()
        {
            if (_buttons.Count is < PopupData.MinButtons or > PopupData.MaxButtons)
            {
                throw new InvalidOperationException(
                    $"Popup needs {PopupData.MinButtons}–{PopupData.MaxButtons} buttons, got {_buttons.Count}.");
            }

            return new PopupData(_title, _body, _buttons.ToArray(), _closeOnButtonClick);
        }

        public PopupView Show(IPopupService service) => service.Show(Build());
    }
}
