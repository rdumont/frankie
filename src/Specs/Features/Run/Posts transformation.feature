Feature: Posts transformation
	In order to share my ideas
	As a site owner
	I want to write posts in markdown format

Background:
	Given the default directory structure
	 And the configuration file
		| Field     | Value                    |
		| Permalink | :year/:month/:day/:title |
	 And the in-memory logger
	 And the '_post' template
		"""
		{{ post.body }}
		"""

Scenario: Transform a post
	Given the '_posts/2013-04-25-some-nice-post.md' text file
		"""
		Here is a paragraph.
		"""
	When I run Frankie
	Then a post with slug "some-nice-post" should be registered
	 And there should be a '_site/2013/04/25/some-nice-post/index.html' text file
         """
		 <p>Here is a paragraph.</p>
         """

Scenario: Extract title from post
	Given the '_posts/2013-04-25-some-nice-post.md' text file
		"""
		# This is a nice post

		And here is a paragraph.
		"""
	When I run Frankie
	Then a post called "This is a nice post" should be registered

Scenario: A post with a category
	Given the '_posts/Food for Thought/2013-04-25-some-nice-post.md' text file
		"""
		This is a post
		"""
	When I run Frankie
	Then the post 'some-nice-post' should have the categories
		| Category         |
		| Food for Thought |

Scenario: A post with many categories
	Given the '_posts/Food for Thought/Acentuação/2013-04-25-some-nice-post.md' text file
		"""
		This is a post
		"""
	When I run Frankie
	Then the post 'some-nice-post' should have the categories
		| Category         |
		| Food for Thought |
		| Acentuação       |

