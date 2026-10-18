using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Providers;
using UnityEngine;
using VContainer;

public class WeaponViewer : MonoBehaviour
{
    [SerializeField] private List<Cannon> _cannons = new();
    [SerializeField] private Cannon _baseCannon;

    private Shooter _shooter;
    private IItemConainerProvider _itemConainerProvider;
    //private IBallContainer _ballContainer;
    private PlayerCardHolder _playerCardHolder;

    public Cannon CurrentCannon { get; private set; }
    public WeaponModel WeaponModel { get; private set; }

    [Inject]
    public void Construct (IItemConainerProvider itemConainerProvider)
    {
        _itemConainerProvider = itemConainerProvider;
    }

    public void Init (Shooter shooter, PlayerCardHolder playerCardHolder)
    {
        _playerCardHolder = playerCardHolder;
        _shooter = shooter;
        //_ballContainer = ballContainer;
    }

    private void Start ()
    {
        _itemConainerProvider.WeaponSeted += ViewCannon;
        _itemConainerProvider.WeaponRemoved += ViewBaseCannon;
        ViewCannon(_itemConainerProvider.Gun);
    }

    //private void OnEnable ()
    //{
    //    if (_itemConainerProvider != null)
    //    {
    //        Debug.Log("OnE");
    //        _itemConainerProvider.WeaponSeted += ViewCannon;
    //        _itemConainerProvider.WeaponRemoved += ViewBaseCannon;
    //    }
    //}

    private void OnDisable ()
    {
        if (_itemConainerProvider != null)
        {
            _itemConainerProvider.WeaponSeted -= ViewCannon;
            _itemConainerProvider.WeaponRemoved -= ViewBaseCannon;
        }
    }

    private void ViewBaseCannon ()
    {
        if (CurrentCannon != null)
            CurrentCannon.gameObject.SetActive(false);

        _baseCannon.gameObject.SetActive(true);
        _playerCardHolder.RemoveItem(WeaponModel);
        _shooter.SetCannon(_baseCannon);

        CurrentCannon = _baseCannon;
        WeaponModel = null;
    }

    private void ViewCannon (WeaponModel weapon)
    {
        if (_itemConainerProvider.Gun == null)
        {
            ViewBaseCannon();
            return;
        }

        if(CurrentCannon != null)
            CurrentCannon.gameObject.SetActive(false);

        foreach (var cannon in _cannons)
        {
            if (cannon.WeaponType == weapon.WeaponConfig.Cannon.WeaponType)
            {
                WeaponModel = weapon;
                cannon.gameObject.SetActive(true);
                _playerCardHolder.AddItem(weapon);
                _shooter.SetCannon(cannon);
                CurrentCannon = cannon;
                return;
            }
        }

        throw new ArgumentNullException(nameof(weapon));
    }
}
