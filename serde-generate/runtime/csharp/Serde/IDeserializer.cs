// Copyright (c) Facebook, Inc. and its affiliates
// SPDX-License-Identifier: MIT OR Apache-2.0

using System.Collections.Immutable;
using System.Numerics;
using System.Text;

namespace Serde
{
    internal interface IDeserializer
    {
        string deserialize_str();

        ImmutableArray<byte> deserialize_bytes();

        bool deserialize_bool();

        Unit deserialize_unit();

        Rune deserialize_rune();

        float deserialize_f32();

        double deserialize_f64();

        byte deserialize_u8();

        ushort deserialize_u16();

        uint deserialize_u32();

        ulong deserialize_u64();

        UInt128 deserialize_u128();

        sbyte deserialize_i8();

        short deserialize_i16();

        int deserialize_i32();

        long deserialize_i64();

        Int128 deserialize_i128();

        long deserialize_len();

        int deserialize_variant_index();

        bool deserialize_option_tag();

        void increase_container_depth();

        void decrease_container_depth();

        int get_buffer_offset();

        void check_that_key_slices_are_increasing(Range key1, Range key2);
    }
}
