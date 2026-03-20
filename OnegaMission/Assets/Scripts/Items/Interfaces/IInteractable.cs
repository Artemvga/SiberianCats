using Player;

namespace Items
{
    /// <summary>
    /// Интерфейс для всех интерактивных объектов в игре.
    /// Позволяет унифицировать работу с подсветкой, информацией и взаимодействием.
    /// </summary>
    public interface IInteractable
    {
        /// <summary> Название предмета (отображается в UI). </summary>
        string ItemName { get; }

        /// <summary> Тип предмета (отображается в UI). </summary>
        string ItemType { get; }

        /// <summary> Описание предмета. </summary>
        string Description { get; }

        /// <summary> Легенда / история предмета. </summary>
        string Lore { get; }

        /// <summary>
        /// Флаг, указывающий, нужно ли показывать сообщение о требовании (например, "Нужен инструмент"),
        /// если взаимодействие в данный момент невозможно.
        /// </summary>
        bool ShouldShowRequirement { get; }

        /// <summary> Вызывается при наведении прицела на объект (если разрешено). </summary>
        void OnFocus();

        /// <summary> Вызывается при потере фокуса. </summary>
        void OnDefocus();

        /// <summary> Действие при нажатии кнопки взаимодействия (обычно E). </summary>
        void Interact();

        /// <summary>
        /// Проверяет, может ли игрок взаимодействовать с объектом, учитывая его инструменты.
        /// </summary>
        bool CanInteract(PlayerTools tools);
    }
}