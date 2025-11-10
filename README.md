# Object Pooling

A complete object pooling system for managing and optimizing the creation and destruction of GameObjects in **C#** for Unity, distributed via the Unity Package Manager (UPM).

Repository: [ObjectPooling](https://github.com/filipeduraes/ObjectPooling).

---

## Features

* **Automatic Management** of object pools for different prefabs.
* **Object Reservation** (Reserve Pool) to pre-instantiate objects and prevent runtime lag.
* **Intelligent Recycling**: Disabled objects are automatically returned to the pool using the `AutomaticPoolReturner`.
* **Container System** to keep the scene hierarchy clean.
* **Asynchronous Return** to the pool to prevent immediate parenting issues. (using `ObjectPoolRoot`).

---

## Installation

### Using UPM (Unity Package Manager)

1.  In Unity, open **Window > Package Manager**.
2.  Click **+** > **Add package from git URL...**.
3.  Enter: `https://github.com/filipeduraes/ObjectPooling.git`.
4.  Unity will fetch and install the package.

### Using `manifest.json`

Alternatively, add this entry to your `Packages/manifest.json`:

```json
{
  "dependencies": {
   "com.ideatogame.object-pooling": "https://github.com/filipeduraes/ObjectPooling.git"
  }
}
```

---

## Usage

### Reserving Objects (Warm-up)

It is recommended to reserve objects at the start of the scene to avoid runtime allocations. Use the method `ReservePool<T>(T prefab, uint reservedAmount, bool useAutomaticReturn = true)`.

```csharp
using IdeaToGame.ObjectPooling;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Enemy enemyPrefab;

    private void Start()
    {
        // Reserves 10 bullets and 5 enemies in the pool
        ObjectPool.ReservePool(bulletPrefab, 10);
        ObjectPool.ReservePool(enemyPrefab, 5);
    }
}
```

### Getting an Object from the Pool

Use `GetFromPool` to get an instance of the prefab. If the pool is empty, a new object will be instantiated and returned. The returned object is set active and its `parent` can be optionally set.

```csharp
using IdeaToGame.ObjectPooling;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform spawnPoint;

    public void Fire()
    {
        // Gets the object, sets it active, and sets 'SpawnPoint' as its parent.
        Bullet bulletInstance = ObjectPool.GetFromPool(bulletPrefab, true, spawnPoint);
        // Add initialization logic here, such as setting velocity, etc.
    }
}
```

### Returning an Object to the Pool

The return can be done in two ways:

1.  **Automatic** (Recommended): If `AutomaticPoolReturner` is present on the object (default if `useAutomaticReturn = true` is used in `GetFromPool` or `ReservePool`), it will automatically return to the pool when the object is disabled (`gameObject.SetActive(false)`).

2.  **Manual**: Explicitly call `ReturnToPool<T>(T prefab, GameObject gameObject)`.

```csharp
using IdeaToGame.ObjectPooling;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Bullet bulletPrefab; // Must be the original prefab reference

    // Example of manual return
    private void OnCollisionEnter(Collision collision)
    {
        // Only for scenarios where you disabled automatic return.
        if (gameObject.GetComponent<AutomaticPoolReturner>() == null)
        {
            ObjectPool.ReturnToPool(bulletPrefab, gameObject);
        }
    }
}
```

---

## ⚙️ How It Works

### `ObjectPool.cs`

The static class that manages the pool dictionaries (`ObjectPools`) using `Queue<Component>` and containers (`Containers`). It coordinates the creation, retrieval, and return of objects.

### `AutomaticPoolReturner.cs`

A `MonoBehaviour` added to the instantiated object. It stores the prefab reference (`_prefab`) and uses Unity's `OnDisable()` method to call `ObjectPool.ReturnToPool` for the `_prefab` and `gameObject`.

### `ObjectPoolRoot.cs`

A `MonoBehaviour` that contains the `SetParentDelayed(Transform child, Transform parent)` method. This method starts a coroutine that waits one frame (`yield return null`) before setting the object's `parent`, preventing hierarchy issues when returning objects.

---

## 📄 License

MIT License
See [LICENSE](LICENSE) for details.
