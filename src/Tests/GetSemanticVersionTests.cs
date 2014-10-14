﻿namespace SemanticGit
{
	using Microsoft.Build.Framework;
	using Moq;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Xunit;

	public class GetSemanticVersionTests
	{
		[Fact]
		public void when_tag_is_semantic_then_parses_major_minor_patch()
		{
			var task = new GetSemanticVersion
			{
				BuildEngine = Mock.Of<IBuildEngine>(),
				Tag = "v1.0.2",
			};

			task.Execute();

			Assert.Equal("1", task.Major);
			Assert.Equal("0", task.Minor);
			Assert.Equal("2", task.Patch);
		}

		[Fact]
		public void when_tag_has_commits_then_adds_them_to_patch()
		{
			var task = new GetSemanticVersion
			{
				BuildEngine = Mock.Of<IBuildEngine>(),
				// This is the format that git describe --tags renders.
				Tag = "v1.0.2-6-g778787d",
			};

			task.Execute();

			Assert.Equal("8", task.Patch);
		}

		[Fact]
		public void when_tag_has_prerelease_prefix_then_parses_it()
		{
			var task = new GetSemanticVersion
			{
				BuildEngine = Mock.Of<IBuildEngine>(),
				// This is the format that git describe --tags renders.
				Tag = "v1.0.2-pre-6-g778787d",
			};

			task.Execute();

			Assert.Equal("-pre", task.PreRelease);
		}

		[Fact]
		public void when_tag_has_prerelease_but_no_commits_on_top_then_patch_matches_tag()
		{
			var task = new GetSemanticVersion
			{
				BuildEngine = Mock.Of<IBuildEngine>(),
				// This is the format that git describe --tags renders.
				Tag = "v1.0.2-pre",
			};

			task.Execute();

			Assert.Equal("2", task.Patch);
		}

		[Fact]
		public void when_non_semantic_tag_then_fails_and_logs()
		{
			var task = new GetSemanticVersion
			{
				BuildEngine = Mock.Of<IBuildEngine>(),
				Tag = "Beta1",
			};

			Assert.False(task.Execute());

			Mock.Get(task.BuildEngine).Verify(x => x.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()));
		}

	}
}
