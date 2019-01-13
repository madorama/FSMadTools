# FSMadTools

VRChat用のUnityエディタ拡張です。

---

## インストール

[こちらから.unitypackageファイルをダウンロード](http://github.com/mamunine/FSMadTools/releases)し、Unity上で`Assets -> Import Package -> Custom Package...`から、もしくは`ダウンロードした.unitypackageファイルをダブルクリック`してインポートしてください。

---

## MadTools/AvatarTools

### Scaling

![Scaling Image](https://user-images.githubusercontent.com/13612643/51085516-d7c6ed80-177d-11e9-83d5-61db7af6904b.png)

1. Hierarchyから`VRC_AvatarDescriptorコンポーネントが存在するGameObject`を選択
  - 選択時、自動的にGameObjectのViewPositionとScaleからDefault ViewPositionが求められます
1. `Default ViewPosition`に`GameObjectのScaleが全て1の時の値`を設定
1. `Scale`に任意の値を設定
1. `Change Scale`ボタンを押す

`GameObjectのScale`を`Scale`の値に変更し、`VRC_Avatar DescriptorのViewPosition`を良い感じに変更します。

### LipSync Attacher

![LipSync Attacher Image](https://user-images.githubusercontent.com/13612643/51085517-d990b100-177d-11e9-8ddc-30ee5924ffc7.png)

1. Hierarchyから`VRC_AvatarDescriptorコンポーネントが存在するGameObject`を選択
1. `Face Mesh`を選択
1. `Attach Lip Sync`ボタンを押す

LipSyncを`Viseme Blend Shape`にし、各Visemeを自動的に設定してくれます。

### Finger Editor

![Finger Editor Image](https://user-images.githubusercontent.com/13612643/51085519-db5a7480-177d-11e9-9380-3a2b3b971389.png)

1. `アニメーションクリップが設定されているGameObject`を選択
1. `Clip`から設定したいアニメーションクリップを選択
1. 各指を設定し、`Apply`ボタンを押す

指の設定が簡単に行えます。
