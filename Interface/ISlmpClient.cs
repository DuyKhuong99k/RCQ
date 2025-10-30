using PLCSlmpEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface  ISlmpClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Ip address</returns>
        public string GetIP();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>port</returns>
        public int GetPort();
        ///// <summary>Initializes a new instance of the <see cref="SlmpClient" /> class.</summary>
        ///// <param name="cfg">The config.</param>
        public void Initialize(ISlmpConfig cfg);

        /// <summary>Connects to the address specified in the config.</summary>
        /// <exception cref="System.TimeoutException">connection timed out</exception>
        public void Connect();

        /// <summary>
        /// Attempt to close the socket connection.
        /// </summary>
        public void Disconnect();
        /// <summary>
        /// Query the connection status.
        /// </summary>
        public bool IsConnected();

        /// <summary>
        /// Issue a `SelfTest` command.
        /// </summary>
        public bool SelfTest();

        /// <summary>
        /// Reads a single Bit from a given `BitDevice` and returns a `bool`.
        /// </summary>
        /// <param name="addr">The device address as a string.</param>
        public bool ReadBitDevice(string addr);

        /// <summary>
        /// Reads from a given `BitDevice` and returns an array of `bool`s.
        /// Note that there's a limit on how many registers can be read at a time.
        /// </summary>
        /// <param name="addr">Start address.</param>
        /// <param name="count">Number of registers to read.</param>
        /// <returns></returns>
        public bool[] ReadBitDevice(string addr, ushort count);

        /// <summary>
        /// Reads a single Bit from a given `BitDevice` and returns a `bool`.
        /// </summary>
        /// <param name="device">The word device.</param>
        /// <param name="addr">Bit address.</param>
        public bool ReadBitDevice(Device device, ushort addr);

        /// <summary>
        /// Reads from a given `BitDevice` and returns an array of `bool`s.
        /// Note that there's a limit on how many registers can be read at a time.
        /// </summary>
        /// <param name="device">The bit device.</param>
        /// <param name="addr">Start address.</param>
        /// <param name="count">Number of registers to read.</param>
        public bool[] ReadBitDevice(Device device, ushort addr, ushort count);

        /// <summary>
        /// Reads a single Word from a the given `WordDevice` and returns an `ushort`.
        /// </summary>
        /// <param name="addr">The device address as a string.</param>
        public ushort ReadWordDevice(string addr);

        /// <summary>
        /// Reads from a given `WordDevice` and returns an array of `ushort`s.
        /// Note that there's a limit on how many registers can be read at a time.
        /// </summary>
        /// <param name="addr">Start address as a string.</param>
        /// <param name="count">Number of registers to read.</param>
        public ushort[] ReadWordDevice(string addr, ushort count);

        /// <summary>
        /// Reads a single Word from a the given `WordDevice` and returns an `ushort`.
        /// </summary>
        /// <param name="device">The word device.</param>
        /// <param name="addr">Word address.</param>
        public ushort ReadWordDevice(Device device, ushort addr);

        /// <summary>
        /// Reads from a given `WordDevice` and returns an array of `ushort`s.
        /// Note that there's a limit on how many registers can be read at a time.
        /// </summary>
        /// <param name="device">The word device.</param>
        /// <param name="addr">Start address.</param>
        /// <param name="count">Number of registers to read.</param>
        public ushort[] ReadWordDevice(Device device, ushort addr, ushort count);

        /// <summary>
        /// Reads a string with the length `len` from the specified `WordDevice`. Note that
        /// this function reads the string at best two chars, ~500 times in a second.
        /// Meaning it can only read ~1000 chars per second.
        /// Note that there's a limit on how many registers can be read at a time.
        /// </summary>
        /// <param name="addr">Starting address of the null terminated string as a string.</param>
        /// <param name="len">Length of the string.</param>
        public string ReadString(string addr, ushort len);

        /// <summary>
        /// Reads a string with the length `len` from the specified `WordDevice`. Note that
        /// this function reads the string at best two chars, ~500 times in a second.
        /// Meaning it can only read ~1000 chars per second.
        /// Note that there's a limit on how many registers can be read at a time.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="addr">Starting address of the null terminated string.</param>
        /// <param name="len">Length of the string.</param>
        public string ReadString(Device device, ushort addr, ushort len);

        /// <summary>
        /// Read from a `WordDevice` to create a C# structure.
        /// The target structure can only contain very primitive data types.
        /// </summary>
        /// <typeparam name="T">The `Struct` to read.</typeparam>
        /// <param name="addr">Starting address of the structure data in the string format.</param>
        public T? ReadStruct<T>(string addr) where T : struct;

        /// <summary>
        /// Read from a `WordDevice` to create a C# structure.
        /// The target structure can only contain very primitive data types.
        /// </summary>
        /// <typeparam name="T">The `Struct` to read.</typeparam>
        /// <param name="device">The device to read from..</param>
        /// <param name="addr">Starting address of the structure data.</param>
        public T? ReadStruct<T>(Device device, ushort addr) where T : struct;

        /// <summary>
        /// Writes a single `Bit` to a given `BitDevice`.
        /// </summary>
        /// <param name="addr">Device address in string format.</param>
        /// <param name="data">Data to be written into the remote device.</param>
        public void WriteBitDevice(string addr, bool data);

        /// <summary>
        /// Writes an array of `bool`s to a given `BitDevice`.
        /// note that there's a limit on how many registers can be written at a time.
        /// </summary>
        /// <param name="addr">Starting address in string format.</param>
        /// <param name="data">data to be written into the remote device.</param>
        public void WriteBitDevice(string addr, bool[] data);

        /// <summary>
        /// Writes a single `Bit` to a given `BitDevice`.
        /// </summary>
        /// <param name="device">The WordDevice to write.</param>
        /// <param name="addr">Address.</param>
        /// <param name="data">Data to be written into the remote device.</param>
        public void WriteBitDevice(Device device, ushort addr, bool data);

        /// <summary>
        /// writes an array of `bool`s to a given `bitdevice`.
        /// note that there's a limit on how many registers can be written at a time.
        /// </summary>
        /// <param name="device">the bitdevice to write.</param>
        /// <param name="addr">starting address.</param>
        /// <param name="data">data to be written into the remote device.</param>
        public void WriteBitDevice(Device device, ushort addr, bool[] data);

        /// <summary>
        /// Writes a single `ushort` to a given `WordDevice`.
        /// </summary>
        /// <param name="addr">Device address in string format.</param>
        /// <param name="data">Data to be written into the remote device.</param>
        public void WriteWordDevice(string addr, ushort data);

        /// <summary>
        /// Writes an array of `ushort`s to a given `WordDevice`.
        /// Note that there's a limit on how many registers can be written at a time.
        /// </summary>
        /// <param name="addr">Starting address in string format.</param>
        /// <param name="data">Data to be written into the remote device.</param>
        public void WriteWordDevice(string addr, ushort[] data);

        /// <summary>
        /// Writes a single `ushort` to a given `WordDevice`.
        /// </summary>
        /// <param name="device">The WordDevice to write.</param>
        /// <param name="addr">Address.</param>
        /// <param name="data">Data to be written into the remote device.</param>
        public void WriteWordDevice(Device device, ushort addr, ushort data);

        /// <summary>
        /// Writes an array of `ushort`s to a given `WordDevice`.
        /// Note that there's a limit on how many registers can be written at a time.
        /// </summary>
        /// <param name="device">The WordDevice to write.</param>
        /// <param name="addr">Starting address.</param>
        /// <param name="data">Data to be written into the remote device.</param>
        public void WriteWordDevice(Device device, ushort addr, ushort[] data);

        /// <summary>
        /// Writes the given string to the specified device as a null terminated string.
        /// Note that there's a limit on how many registers can be written at a time.
        /// </summary>
        /// <param name="addr">Starting address in string format.</param>
        /// <param name="text">The string to write.</param>
        public void WriteString(string addr, string text);

        /// <summary>
        /// Writes the given string to the specified device as a null terminated string.
        /// Note that there's a limit on how many registers can be written at a time.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="addr">Starting address.</param>
        /// <param name="text">The string to write.</param>
        public void WriteString(Device device, ushort addr, string text);
    }
}
