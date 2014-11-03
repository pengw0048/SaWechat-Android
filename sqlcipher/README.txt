这里记录了如何Build在Windows中能够正常打开EnMicroMsg.db的sqlcipher，以及解密该文件的方法。

我们使用sqlcipher-windows这个库，地址为https://github.com/CovenantEyes/sqlcipher-windows。然而经过试验，在所有的四个Release版本中，只有2.0.6可以正常工作。这可能是Android上的微信使用了相似的旧版本库导致的。

从https://github.com/CovenantEyes/sqlcipher-windows/archive/v2.0.6.zip下载这个版本的src。作者使用的是VS2010，所以如果你用旧版本的VS，需要调整“平台工具集”。此外，我们还需要openssl的库。

你可以从官方处下载openssl并自行编译，不过可以在http://www.npcglib.org/~stathis/blog/precompiled-openssl/找到已经编译好的版本。无论怎样，请按照sqlcipher-windows首页中的说明放置编译好的openssl库。为了让你的生活更简单，不妨只保留x86的版本。

好了，现在你已经可以生成sqlcipher了。把它和对应版本的libeay32xx.dll放在一起。

这样得到的sqlcipher，缺少一个实用的函数，即sqlcipher_export()。不过没关系，我们可以用另一种方法实现解密。

sqlcipher.exe EnMicroMsg.db
> PRAGMA key='xxxxxxx';
> cipher_use_hmac=off;
> .output a.sql
> .dump
> .quit
sqlcipher.exe MicroMsg.db
> .read a.sql
> .quit

这样就可以了。至于这个数据库的密钥，即上面的7个x，用以下方法计算：

md5(IMEI+uid).substring(0,7)

在手机拨号界面输入*#06#来获得IMEI。据称，在某些机型上，可能会使用IMSI等其他数字。

uid为微信的用户编号。你可以通过二进制方式扫描/data/data/com.tencent.mm/MicroMsg/***/下的文件，并且逐一验证来获得；或者登录网页版微信，然后在cookies里找wxuid一项。

这可能就是我们需要的所有细节。

或者你只是想拿走现成的sqlcipher.exe。
