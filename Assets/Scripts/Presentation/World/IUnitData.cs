using Logic.Data.World;
using UnityEngine;

namespace Presentation.World {
/// <summary>
///     Extends the <see cref="IUnitTypeData"/> interface with visualization-related properties.
/// </summary>
public interface IUnitData : IUnitTypeData {
	public Color BlueColor { get; }
	public Color RedColor { get; }
	public Sprite PreviewSprite { get; }
	public Sprite AliveSpriteConstant { get; }
	public Sprite AliveSpriteColored { get; }
	public bool Airborne { get; }
}
}
