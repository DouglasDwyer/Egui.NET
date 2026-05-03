#pragma warning disable

// Copyright (c) Facebook, Inc. and its affiliates
// SPDX-License-Identifier: MIT OR Apache-2.0

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Serde
{
    internal abstract class BinarySerializer : ISerializer, IDisposable
    {
        protected readonly MemoryStream buffer;
        protected readonly BinaryWriter output;
        protected readonly Encoding utf8 = Encoding.GetEncoding("utf-8", new EncoderExceptionFallback(), new DecoderExceptionFallback());

        public BinarySerializer()
        {
            buffer = new MemoryStream();
            output = new BinaryWriter(buffer);
        }

        public BinarySerializer(byte[] bufferArray) : this(new ArraySegment<byte>(bufferArray)) { }

        public BinarySerializer(ArraySegment<byte> bufferArray)
        {
            buffer = new MemoryStream(bufferArray.Array, bufferArray.Offset, bufferArray.Count);
            output = new BinaryWriter(buffer);
        }

        public void Reset() {
            buffer.SetLength(0);
        }

        public void Dispose() => output.Dispose();

        public void increase_container_depth()
        {
        }

        public void decrease_container_depth()
        {
        }

        public abstract void serialize_len(long len);

        public abstract void serialize_variant_index(int value);

        public abstract void sort_map_entries(int[] offsets);

        public void serialize_rune(Rune value)
        {
            Span<byte> charBytes = stackalloc byte[4];
            var len = value.EncodeToUtf8(charBytes);
            output.Write(charBytes[..len]);
        }

        public void serialize_f32(float value) => output.Write(value);

        public void serialize_f64(double value) => output.Write(value);

        public ReadOnlySpan<byte> get_bytes() => new ReadOnlySpan<byte>(buffer.GetBuffer(), 0, (int)buffer.Length);

        public void serialize_str(string value) => serialize_bytes(utf8.GetBytes(value).ToImmutableArray());

        public void serialize_bytes(ImmutableArray<byte> value)
        {
            serialize_len(value.Length);
            foreach (byte b in value)
                output.Write(b);
        }

        public unsafe void serialize_seq_unmanaged<T>(ReadOnlySpan<T> value) where T : unmanaged
        {
            serialize_len(value.Length);
            fixed (T* ptr = value)
            {
                output.Write(new ReadOnlySpan<byte>((byte*)ptr, sizeof(T) * value.Length));
            }
        }

        public void serialize_bool(bool value) => output.Write(value);

        public void serialize_unit(Unit value) { }

        public void serialize_u8(byte value) => output.Write(value);

        public void serialize_u16(ushort value) => output.Write(value);

        public void serialize_u32(uint value) => output.Write(value);

        public void serialize_u64(ulong value) => output.Write(value);

        public void serialize_u128(UInt128 value)
        {
            output.Write((ulong)value);
            output.Write((ulong)(value >> 64));
        }

        public void serialize_i8(sbyte value) => output.Write(value);

        public void serialize_i16(short value) => output.Write(value);

        public void serialize_i32(int value) => output.Write(value);

        public void serialize_i64(long value) => output.Write(value);

        public void serialize_i128(Int128 value)
        {
            output.Write((ulong)value);
            output.Write((ulong)((UInt128)value >> 64));
        }

        public void serialize_option_tag(bool value) => output.Write(value);

        public int get_buffer_offset() => (int)output.BaseStream.Position;
    }
}
