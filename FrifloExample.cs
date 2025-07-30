using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace FrifloExample;

class FrifloExample {
	public static void Run() {
		var world = new EntityStore();
		world.EventRecorder.Enabled = true;

		var root = new SystemRoot(world) {
			new OnAnimatorAdded(),
			new ProgressFrame(),
			new DrawSprites(),
		};

		for (int i = 0; i < 3; i++) {
			world.CreateEntity(new Animator());
			root.Update(default);
			world.EventRecorder.ClearEvents();
		}
	}
}

class OnAnimatorAdded : QuerySystem {
	private ArchetypeQuery ListenQuery;

	protected override void OnAddStore(EntityStore store) {
		ListenQuery = store.Query();
		ListenQuery.EventFilter.ComponentAdded<Animator>();
	}

	protected override void OnUpdate() {
		var cmds = CommandBuffer;
		foreach (var entt in ListenQuery.Entities) {
			if (ListenQuery.HasEvent(entt.Id)) {
				cmds.AddComponent(entt.Id, new Sprite());
				cmds.AddTag<IsDrawable>(entt.Id);
			}
		}
		cmds.Playback();
	}
}

class ProgressFrame : QuerySystem<Animator, Sprite> {
	protected override void OnUpdate() {
		Query.ForEachEntity((ref Animator anim, ref Sprite sprite, Entity entt) => {
			anim.frame++;
			sprite.frame = $"{anim.frame}.png";
		});
	}
}

class DrawSprites : QuerySystem<Sprite> {
	public DrawSprites() => Filter.AnyTags(Tags.Get<IsDrawable>());
	protected override void OnUpdate() {
		Query.ForEachEntity((ref Sprite s, Entity entt) => {
			Console.WriteLine($"Render frame {s.frame}");
		});
	}
}

struct Animator() : IComponent {
	public int frame = 0;
}
struct Sprite() : IComponent {
	public string frame = "0.png";
}
struct IsDrawable : ITag;