-> metro_thoughts

=== metro_thoughts ===
    # change_image:metro
    # play_sound:dowsing
    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur."
*   [Choice 1]
    # change_image:tar_hand
    # play_sound:casual_loop
    Now we switched to another scene, we heard a button click and music changed, as well as the background image.
    -> continue_reflection

*   [Choice 2]
    # change_image:woman
    # play_sound:casual_loop
    Now we switched to another scene, we heard a button click and music changed, as well as the background image.
    -> continue_reflection

= continue_reflection
    Now this sentence is a 'knot', and its here to demostrate the system. After both the Choice 1 and 2, this sentence moved the story forward.
    *   [Choice 3]
        This choice leads to the bad ending.
        -> metro_conclusion_bad
        
    *   [Choice 4]
        This choice leads to the good ending.
        -> metro_conclusion_good

= metro_conclusion_bad
    # change_image:arms
    # play_sound: pneumatic
    # lerp_preset
    This is the knot for bad ending text. Music and background changed again. And look, there's shader magic going on as well!
    -> END

= metro_conclusion_good
    # change_image:hacker
    # play_sound: pneumatic
    # lerp_preset
    This is the knot for good ending text. Music and background changed again. And look, there's shader magic going on as well!
    -> END