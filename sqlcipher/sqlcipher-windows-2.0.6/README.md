SQLCipher for Visual Studio
===========================

[sqlcipher-windows][] is a simple port of the open-source [SQLCipher][]
library [[src](https://github.com/sqlcipher/sqlcipher)] for Visual Studio 2010.

The base source code is copied from [SQLite3][] (amalgamation form). The
security enhancements provided by SQLCipher are copied directly from its
source code. This is trivial since SQLCipher's modifications to SQLite's source
are denoted by code enclosed in

    /* BEGIN CRYPTO */
    ...
    /* END CRYPTO */

sqlcipher-windows adds a few minor code modifications to allow SQLCipher's
code to build with the SQLite amalgamation source distribution and to quell
some warnings.

This project was mostly informed by the generous video by Mike Stephenson which
can be found
[here](https://groups.google.com/forum/?fromgroups#!topic/sqlcipher/WJZVs7ydk2o).


  [sqlcipher-windows]: https://github.com/CovenantEyes/sqlcipher-windows
  [SQLCipher]: http://sqlcipher.net/
  [SQLite3]: http://www.sqlite.org/


Dependencies
------------

You will need a build of [OpenSSL][] as a static library for x86 and x86-64
(see [Building OpenSSL for Visual Studio][]). Place the OpenSSL libraries in a
folder structured like this:

    openssl\
        x86\
            lib\
                libeay32.lib
                libeay32-debug.lib
            openssl\
                [openssl headers]
        x86-64\
            lib\
                libeay64.lib
                libeay64-debug.lib
            openssl\
                [openssl headers]

Place the path to your `openssl` folder in a Windows environment variable
called `OpenSslDir`. For example, if `C:\Libraries\openssl-1.0.1c` is the path
to your OpenSSL library folder, then `OpenSslDir` must be
`C:\Libraries\openssl-1.0.1c`.

  [OpenSSL]: http://www.openssl.org/
  [Building OpenSSL for Visual Studio]: http://developer.covenanteyes.com/building-openssl-for-visual-studio/


Versions
--------

`sqlcipher-windows` is a combination of

  * [SQLite 3.7.13 (amalgamation)](http://www.sqlite.org/sqlite-amalgamation-3071300.zip)
  * [SQLCipher 2.0.6](https://github.com/sqlcipher/sqlcipher/zipball/v2.0.6)

and has been successfully built with the following versions of OpenSSL:

   * [OpenSSL 1.0.1c](http://www.openssl.org/source/openssl-1.0.1c.tar.gz)
