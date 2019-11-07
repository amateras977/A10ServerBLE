# A10ServerBLE


Vorze製電動オナホールをRESTAPIで制御できるWebサーバ

- 対応デバイス
  - [A10ピストンSA](https://www.vorze.jp/a10pistonsa/)
  - [A10サイクロンSA](https://www.vorze.jp/a10cyclonesa/)
  - [UFOSA](https://www.vorze.jp/ufosa/)




## 使い方

単体でも使用可能だが、下記pluginとの連携を想定して設計している

[KoikatsuA10](https://github.com/amateras977/KoikatsuA10)


### 用意するもの

- PC
  - Windows10 バージョン1903以降
- 対象デバイスを1台以上
  - A10サイクロンSA
  - A10ピストンSA
  - UFOSA
- Bluetoothアダプタもしくは、Bluetoothに接続できるPC
  - 要Bluetooth 4.0対応(BLEを使用するため)

- [専用無線アダプタ](https://www.e-nls.com/pict1-41903?c2=9999) を使用する場合は[こちら](https://github.com/amateras977/A10Server)

### インストール

- 最新版のA10ServerBLEをダウンロードする
  - [Releases](https://github.com/amateras977/A10ServerBLE/releases)
- A10ServerBLE.exeを実行
- 接続対象のデバイスの電源を入れる
  - 起動したものすべてに自動接続
- ブラウザからこのURLを開き、デバイスが1発動けばインストール完了
  - [テスト用URL](http://localhost:8080/api/addQueue?interval=0.3&direction=1)

## RESTAPI

/api/addQueue

ストローク操作のキュー予約を追加する

- パラメータ
  - interval
    - ストローク1回にかける目安時間(sec)
  - direction
    - ストロークの方向。1が奥、-1が手前

/api/clearQueue

ストローク操作の予約キューを消去する

## ビルド方法

.Net Core 3.0をビルドでき、dotnetコマンドを利用できる環境を用意すること。
VisualStudio 2019を推奨

このリポジトリを適当なディレクトリに展開し、リポジトリの直下でこのコマンドを実行

```
dotnet publish -r win10-x64 -c Release --self-contained true
```
