using Flecs.NET.Core;

namespace FlecsExample;

class FlecsExample {
	public static void Run() {
		World world = World.Create();
		world.Import<RenderModule>();
		for (int i = 0; i < 3; i++) {
			world.Entity().Set(new Animator());
			world.Progress();
		}
	}
}

public struct RenderModule : IFlecsModule {
	public void InitModule(World world) {
		world.Observer<Animator>()
			.Event(Ecs.OnSet)
			.Each((Entity entt, ref Animator a) => {
				entt.Set(new Sprite());
				entt.Add<IsDrawable>();
			});

		world.System<Animator, Sprite>("ProgressFrame")
			.Kind(Ecs.OnUpdate)
			.Each((ref Animator anim, ref Sprite sprite) => {
				anim.frame++;
				sprite.frame = $"{anim.frame}.png";
			});

		world.System<Sprite>("DrawSprites")
			.With<IsDrawable>()
			.Kind(Ecs.OnUpdate)
			.Each((ref Sprite s) => Console.WriteLine($"Render frame {s.frame}"));
	}
}

struct Animator() {
	public int frame = 0;
}
struct Sprite() {
	public string frame = "0.png";
}
enum IsDrawable;