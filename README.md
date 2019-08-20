# FSMadTools

VRChat用のUnityエディタ拡張です。

---

## インストール

[こちらから.unitypackageファイルをダウンロード](http://github.com/madorama/FSMadTools/releases)し、Unity上で`Assets -> Import Package -> Custom Package...`から、もしくは`ダウンロードした.unitypackageファイルをダブルクリック`してインポートしてください。
インポートすると、Unityのメニューバーに`MadTools`が追加されます。

---

## 使い方
Unityのメニューバーから`MadTools`を選択してください。
各ツールの使い方は以下を参照してください。

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

![Finger Editor Image](https://user-images.githubusercontent.com/13612643/63315169-20164180-c345-11e9-87f6-442b898627f0.png)

1. `アニメーションクリップが設定されているGameObject`を選択
1. `Clip`から設定したいアニメーションクリップを選択
1. 各指を設定し、`Apply`ボタンを押す

指の設定が簡単に行えます。

#### テンプレート機能

Finger Editorにはテンプレート機能があります。

よく使う指の形を保存しておける感じの機能です。

`Save`を押すと、`選択しているテンプレートに現在の指設定(右側のやつ)を保存`します。

`Load`を押すと、`選択しているテンプレートに保存されている指設定を読み込み`ます。

`Add`を押すと、`新しいテンプレートを追加`します。

`Remove`を押すと、`選択しているテンプレートを削除`します。

`Add`、`Remove`の動作は`Save`を押されるまで保存されることはありません。
間違ってテンプレートを`Remove`してしまった場合、`タブを切り替える`か`AvatarToolsウィンドウ`を閉じてください。
