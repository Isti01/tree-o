using Logic.Data.World;
using UnityEngine;

namespace Presentation.World {
/// <summary>
///     Extends the <see cref="ITowerTypeData"/> interface with visualization-related properties.
/// </summary>
public interface ITowerData : ITowerTypeData {
	public Sprite PreviewSprite { get; }
	public Sprite SpriteColored { get; }
	public Sprite SpriteConstant { get; }
	public Sprite SpriteBackground { get; }
	public Color BlueColor { get; }
	public Color RedColor { get; }
}
}
