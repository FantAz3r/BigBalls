using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class PointerEventDispatcher : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler,
    IScrollHandler,
    IInitializePotentialDragHandler,
    ISelectHandler,
    IDeselectHandler,
    ISubmitHandler,
    ICancelHandler
{
    private readonly Dictionary<System.Type, List<Component>> _cache = new();


    private List<Component> GetHandlers<T> () where T : class
    {
        var type = typeof(T);
        if (_cache.TryGetValue(type, out var list))
            return list;

        list = new List<Component>();
        var all = GetComponents<MonoBehaviour>();

        foreach (var mb in all)
        {
            if (mb != null && mb != this && mb is T)
            {
                list.Add(mb);
                Debug.Log(mb);
            }
        }

        _cache[type] = list;
        return list;
    }

    public void InvalidateCache () => _cache.Clear();


    private void Dispatch<T> (System.Action<T> action) where T : class
    {
        var handlers = GetHandlers<T>();

        for (int i = 0; i < handlers.Count; i++)
        {
            if (handlers[i] == null)
                continue;

            if (!handlers[i].gameObject.activeInHierarchy == false)
                continue;

            action((T) (object) handlers[i]);
        }
    }


    public void OnPointerEnter (PointerEventData e) => Dispatch<IPointerEnterHandler>(h => h.OnPointerEnter(e));
    public void OnPointerExit (PointerEventData e) => Dispatch<IPointerExitHandler>(h => h.OnPointerExit(e));
    public void OnPointerDown (PointerEventData e) => Dispatch<IPointerDownHandler>(h => h.OnPointerDown(e));
    public void OnPointerUp (PointerEventData e) => Dispatch<IPointerUpHandler>(h => h.OnPointerUp(e));
    public void OnPointerClick (PointerEventData e) => Dispatch<IPointerClickHandler>(h => h.OnPointerClick(e));

    public void OnInitializePotentialDrag (PointerEventData e)
        => Dispatch<IInitializePotentialDragHandler>(h => h.OnInitializePotentialDrag(e));
    public void OnBeginDrag (PointerEventData e) => Dispatch<IBeginDragHandler>(h => h.OnBeginDrag(e));
    public void OnDrag (PointerEventData e) => Dispatch<IDragHandler>(h => h.OnDrag(e));
    public void OnEndDrag (PointerEventData e) => Dispatch<IEndDragHandler>(h => h.OnEndDrag(e));
    public void OnDrop (PointerEventData e) => Dispatch<IDropHandler>(h => h.OnDrop(e));

    public void OnScroll (PointerEventData e) => Dispatch<IScrollHandler>(h => h.OnScroll(e));

    public void OnSelect (BaseEventData e) => Dispatch<ISelectHandler>(h => h.OnSelect(e));
    public void OnDeselect (BaseEventData e) => Dispatch<IDeselectHandler>(h => h.OnDeselect(e));
    public void OnSubmit (BaseEventData e) => Dispatch<ISubmitHandler>(h => h.OnSubmit(e));
    public void OnCancel (BaseEventData e) => Dispatch<ICancelHandler>(h => h.OnCancel(e));
}