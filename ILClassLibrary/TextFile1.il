.module Iot.Device.ILTesting.dll

.assembly extern mscorlib
{
  .publickeytoken = (C0 7D 48 1E 97 58 C7 31 )                         // .}H..X.1
  .ver 1:15:6:0
}

.assembly Iot.Device.ILTesting
{
  .custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 1E 2E 4E 45 54 6E 61 6E 6F 46 72 61 6D 65   // ....NETnanoFrame
                                                                                                        77 6F 72 6B 2C 56 65 72 73 69 6F 6E 3D 76 31 2E   // work,Version=v1.
                                                                                                        30 01 00 54 0E 14 46 72 61 6D 65 77 6F 72 6B 44   // 0..T..FrameworkD
                                                                                                        69 73 70 6C 61 79 4E 61 6D 65 16 2E 4E 45 54 20   // isplayName..NET 
                                                                                                        6E 61 6E 6F 46 72 61 6D 65 77 6F 72 6B 20 31 2E   // nanoFramework 1.
                                                                                                        30 )
}

.class public auto ansi sealed beforefieldinit Iot.Device.TestClass
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{

.method public hidebysig specialname rtspecialname 
        instance void  .ctor() cil managed
{
  // Code size       21 (0x15)
  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
  IL_0014:  ret
} // end of method Graphics::.ctor

.method public instance string Foo() cil managed
{
	ldstr "Hello World";
	ret;
}

} // end of class Iot.Device.EPaper.Graphics
