Feature: Markdown page transformation
	In order to save time on writing complex markup
	As a site owner
	I want to generate a page using Markdown

Background:
	Given the default directory structure
	 And the in-memory logger

Scenario: Page with default template
	Given the '_page' template
		"""
		This is a page.
		{{ content }}
		"""
	 And the 'my-page.md' text file
		"""
		**My page**
		"""
	When I run Frankie
	Then there should be a '_site/my-page.html' text file
		"""
		This is a page.
		<p><strong>My page</strong></p>
		"""
	 And no errors should be logged

Scenario: Page with custom template
	Given the 'layout' template
		"""
		This is the layout.
		{{ content }}
		"""
	 And the 'my-page.md' text file
		"""
		@template layout
		**My page**
		"""
	When I run Frankie
	Then there should be a '_site/my-page.html' text file
        """
        This is the layout.
		<p><strong>My page</strong></p>
        """
	 And no errors should be logged

Scenario: Page inside folder
	Given the '_page' template
		"""
		{{ content }}
		"""
	 And the 'about/me.md' text file
		"""
		# About me
		"""
	When I run Frankie
	Then there should be a '_site/about/me.html' text file
		"""
		<h1 id="about-me">About me</h1>
		"""
	 And no errors should be logged

Scenario: Page with embedded liquid syntax
	Given the '_page' template
		"""
		{{ content }}
		"""
	 And the 'gravatar' template
		"""
		This is a **gravatar** include
		"""
	 And the 'liquid-page.md' text file
		"""
		The page.

		{% include gravatar %}
		"""
	When I run Frankie
	Then there should be a '_site/liquid-page.html' text file
		"""
		<p>The page.</p>
		<p>This is a <strong>gravatar</strong> include</p>
		"""
	 And no errors should be logged

Scenario: Code block with language
	Given the '_page' template
		"""
		{{ content }}
		"""
	And the 'code-page.md' text file
		"""
		Some code.

		```csharp
		public class Foo { }
		```
		"""
	When I run Frankie
	Then there should be a '_site/code-page.html' text file
		"""
		<p>Some code.</p>
		<pre><code data-language="csharp">public class Foo { }
		</code></pre>
		"""
	 And no errors should be logged