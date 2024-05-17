VAR has_seen_think_about_murder = false
VAR has_seen_think_about_zoe = false
VAR has_seen_hale_giving_advice = false
VAR has_expressed_anger = false
VAR has_moved_on = false

-> metro_thoughts

=== metro_thoughts ===
    # change_image:metro
    # play_sound:dowsing
    In times like this, I can't help but think about staying in orbit instead of accepting all this. When has pursuing the ghosts of the past ever benefited anyone?
    * [Continue]
        We come to a stop at Taksim, and in an instant, a wave of people enters the metro. "It's late, and still too crowded. I just hope Zane cares for his privacy, otherwise things will get ugly."
        ** [Continue]
            -> focus_on_the_matter_at_hand

= focus_on_the_matter_at_hand
    # change_image:metro
    "I am getting distracted. Just focus on the matter at hand."
    * [Think about the murder]
        ~ has_seen_think_about_murder = true
        # change_image:tar_hand
        "All those poor people... Killed and torn apart, then grafted into each other." I look at my hands, "We have the same thing. But the question is, is what he's doing just a sick fantasy, or is he working for someone else?"
        ** [Think about the symbol]
            # change_image:arms
            "That is why I first heard him called 'The Fragment'. He takes pieces from his victims and grafts them together into this unified shape."
        *** [Continue]
            I made Zoe look it up, but it doesn't match with anything — no logo, no marking. Can he be building something?"
            **** [Continue to think]
                -> focus_on_the_matter_at_hand
            **** [Move on]
                -> arrived_at_the_scene

    * [Think about Zoe]
        ~ has_seen_think_about_zoe = true
        # change_image:hacker
        It's very difficult right now, and I can only hope that he didn't harm her. I can't believe I couldn't protect her from his grasp. But I'm coming. I will save you.
        ** [Think about her involvement]
            I was very lucky to find someone like her in this city. It's hard to come by good people, and even harder among hackers.
        *** [Continue]
            She aims to use all that genius for the betterment of all people. And I won't allow a psycho to take her away from me.
            **** [Continue to think]
                -> focus_on_the_matter_at_hand
            **** [Move on]
                -> arrived_at_the_scene
            
    * [Move on]
    ~ has_moved_on = true
        -> arrived_at_the_scene

=== arrived_at_the_scene ===
    # change_image:street
    I step away from the metro and start walking quickly to the ground level. A cold gush of wind hits my face, dismissing all the weariness from me. I hear my earpiece ringing; the caller is Dr. Hale. I must pick this up.
    * [Answer the call]
        "Son, I heard about your situation," he says. "I know how difficult the circumstances are there for you, and that meeting with your mom didn't really help either."
    
    ** [Express your anger]
        ~ has_expressed_anger = true
        # play_sound:casual_loop
        As if I wasn't tense enough, that remark he made about my mother was just another log on the fire. "Listen, Adrian, I don't want to hear your 'I was right all along' bickering right now. I'd rather be on my way, and don't bring up my mom again." And I end the call.
        *** [Continue]
        -> observe_the_environment
    
    ** [Listen to him]
        "I just thought she would at least be happy to see me. But anyway, it's a waste to think about that now. You said you heard of the situation; do you have any advice for me?"
        *** [Continue]
            -> hale_giving_advice
    
    = hale_giving_advice
        ~ has_seen_hale_giving_advice = true
        "Yes, I was able to gather some data on 'The Fragment.' As you know, he's trying to make all his enemy gangs submit to him with his power, but there's a catch. I captured a phone call of him with a woman, but even though I was able to deduce that the other caller was a woman, I wasn't able to specify her identity."
        * "What's your theory?"
            # change_image:woman
            # play_sound:casual_loop
            "I don't think the woman is in Istanbul. Or even on Earth. I think she's someone among us. So, if you can, do not kill him. He might be the key to your investigation."
        ** [Thank the doctor]
            "That's quite a claim, Doc. Thank you, I will keep that in mind." And I end the call.
            *** [Continue]
                -> observe_the_environment
    
    = observe_the_environment
    # change_image:street
    # play_sound:dowsing
    As I keep walking towards Zane's decrepit warehouse, I start to observe my surroundings. It is one of the more industrial areas of the city, with textile ateliers lining the streets.
    * [Continue]
        There's a rancid smell in the air, a leftover from the weapons used back in the day. The chemical wastes can still be seen on the walls, around the corners, pavements, everywhere... Poisoning the people every second.
    ** [Continue]
        As I get closer, I start to feel a rush of adrenaline. For one part of me, this is a competition, a chance to show that I am the master of this power. But this feeling is what made Zane into the monster he is now. I am different. I won't succumb to it.
    *** [Think about the mission]
        # change_image:father
        # play_sound:casual_loop
        The drive to search for my father to get answers has led me to this day. But I feel like I am in the middle of a much bigger game now. Am I ready for it?
        **** [Move on]
            -> confrontation
    *** [Move on]
        -> confrontation
    
    === confrontation ===
    # change_image:warehouse
    # play_sound:casual_loop
    I arrive at the warehouse. To my surprise, there is no security around. The entrance door is half-open. I push open the heavy metal door, its shrieking voice hurting my ears. A beam of light from outside enters the building.
    * [Continue]
        I step inside the building, and I feel like my blood boils. I sense flesh in every inch of this room, flesh and blood of different people. I take a couple more steps, and I see a familiar face.
    ** [Take a look]
        # change_image:zane
        I see Zoe, wrapped in black tar and flesh. Then, Victor Zane shows himself.
        "You feel the pulse too, right? Not just here, all around the city," he says. I don't respond right away. I just look at him.
    *** [Keep listening]
        # play_sound:pneumatic
        "You are an outsider. You were born here, but you don't belong here. Istanbul hasn't shaped you; the suits in orbit did. This city would eat you alive. Leave it."
        -> confront_choices
        
        = confront_choices
        * {has_expressed_anger and has_moved_on} 
        [Cut the crap! (Attack him)]
            -> very_bad_ending
            
        * You are making a mistake.
            "Yes, I might be an outsider but I know that you are being used. And the ones that are using you will cut you like you did to these poor people and throw you away."
            ** [Continue]
            # change_image:tar_hand
                "It doesn't matter! As long as I carry this power, I would crush their empire on top of their heads! And you... You have no strength to stop me."
                *** [Continue]
                    He tightens his grip on Zoe, with an evil grin on his face. "Look! This is how you thought you could get to me?"
                    This was it. I will tear this man apart, I think to myself. But then, I see Zoe shaking her head at me, and I understand what that means.
                    **** [Control him with your voice]
                    # change_image:zane
                        He will show resistance, but I have to try. I look him in the eye, and confidently say, "Loosen your fingers." And I see the animated flesh loosening up on Zoe's neck.
                        ***** [Continue]
                            "Do not look away. Kneel!" I said. For a brief moment, he lost control. This was my chance.
                            ****** [Kill him]
                                -> bad_ending
                            ****** [Capture him]
                                -> good_ending
        = very_bad_ending
        # change_image:zane_death
        I jump onto Zane, but from the puddles on the ground, two arms of flesh grab me. As the arms pin me down, Zane walks into my face and makes a grabbing motion with his hand. An awful cracking sound fills the room. I understand what happened and scream at the top of my lungs... She is dead.
        -> END

        = bad_ending
        # change_image:zane_death
        I couldn't resist the temptation. The anger. I turn my hand into a giant sharp blade, formed with flesh. And I slice him. In a blink of an eye, he is dead.
        * [Continue]
        # change_image:hacker
            "Why did you do that?" Zoe asks me. "We could've learned from him. Now his employers will hide so easily. I expected better from you, Erra."
            "You are right. I am sorry. I was angry, and I couldn't manage it."
            -> END

        = good_ending
        # change_image:zane_taken
        He lets go of Zoe and she immediately jumps from her chair onto him. As she is holding him in a choke position, I tie him up.
        *[Continue]
        # play_sound:dowsing
        # change_image:hacker
        "Took you long enough. He really did go mad. He didn't even bring his security. Whatever that 'Vicissitude' thing is, it was controlling him."
        ** [Continue]
        # change_image:zane_taken
            "Damn girl, you've just been out of captivity. Take a breath. We will need a car. Let's see what he knows more..."
            -> END