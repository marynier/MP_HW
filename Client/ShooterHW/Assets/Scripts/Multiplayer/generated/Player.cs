// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.46
// 

using Colyseus.Schema;

public partial class Player : Schema {
	[Type(0, "uint8")]
	public byte skin = default(byte);

	[Type(1, "number")]
	public float gun = default(float);

	[Type(2, "uint8")]
	public byte loss = default(byte);

	[Type(3, "int8")]
	public sbyte maxHP = default(sbyte);

	[Type(4, "int8")]
	public sbyte currentHP = default(sbyte);

	[Type(5, "boolean")]
	public bool crouch = default(bool);

	[Type(6, "number")]
	public float speed = default(float);

	[Type(7, "number")]
	public float pX = default(float);

	[Type(8, "number")]
	public float pY = default(float);

	[Type(9, "number")]
	public float pZ = default(float);

	[Type(10, "number")]
	public float vX = default(float);

	[Type(11, "number")]
	public float vY = default(float);

	[Type(12, "number")]
	public float vZ = default(float);

	[Type(13, "number")]
	public float rX = default(float);

	[Type(14, "number")]
	public float rY = default(float);
}

