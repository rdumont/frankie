Feature: Posts transformation
	In order to share my ideas
	As a site owner
	I want to write posts in markdown format

Background:
	Given the default directory structure
	 And the in-memory logger
	 And the '_post' template
		"""
		{{ post.body }}
		"""

Scenario: Transform a post
	Given the configuration file
		| Field     | Value                    |
		| Permalink | :year/:month/:day/:title |
	 And the '_posts/2013-04-25-some-nice-post.md' text file
		"""
		# This is a nice post

		And here is a paragraph.
		"""
	When I run Frankie
	Then a post with slug "some-nice-post" should be registered
	 And there should be a '2013/04/25/some-nice-post/index.html' text file
         """
         <h1 id="this-is-a-nice-post">This is a nice post</h1>
		 <p>And here is a paragraph.</p>
         """
