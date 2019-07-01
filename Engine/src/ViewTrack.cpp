#include "components.h"
#include "extern.h"
#include "Game.h"

ViewTrack::ViewTrack() {

    ratio = 0;
}

void ViewTrack::render(GLuint prog) {
    
    lm::mat4 vp = ECS.getComponentInArray<Camera>(Game::instance->camera_system_.GetOutputCamera()).view_projection;

    glUseProgram(prog);
    GLint u_mvp = glGetUniformLocation(prog, "u_mvp");
    GLint u_color = glGetUniformLocation(prog, "u_color");
    GLint u_color_mod = glGetUniformLocation(prog, "u_color_mod");
    GLint u_size_scale = glGetUniformLocation(prog, "u_size_scale");
    GLint u_center_mod = glGetUniformLocation(prog, "u_center_mod");

    float grid_colors[12] = {
    0.7f, 0.7f, 0.7f, //grey
    1.0f, 0.5f, 0.5f, //red
    0.5f, 1.0f, 0.5f, //green
    0.5f, 0.5f, 1.0f }; //blue

    {
        //set uniforms and draw grid
        glUniformMatrix4fv(u_mvp, 1, GL_FALSE, vp.m);
        glUniform3fv(u_color, 4, grid_colors);
        glUniform3f(u_size_scale, 1.0, 1.0, 1.0);
        glUniform3f(u_center_mod, 0.0, 0.0, 0.0);
        glUniform1i(u_color_mod, 3);
        glBindVertexArray(curve_vao_); //TRACKS
        glDrawElements(GL_LINES, 33, GL_UNSIGNED_INT, 0);
    }
}

// Update the camera with the track position
void ViewTrack::update(float dt) {

    if (active)
    {
        // Update ratio and get interpolated position along the spline.
        ratio += dt * speed;
        lm::vec3 new_pos = curve.evaluateAsCatmull(ratio);

        // Update camera position here
        Camera& cam = ECS.getComponentFromEntity<Camera>(owner);
        Transform& trans = ECS.getComponentFromEntity<Transform>(owner);

        // Apply transforms and reset ratio
        {
            trans.position(new_pos);
            cam.position = new_pos;
            cam.lookAt(new_pos, cam.target);
            ratio = ratio >= 1 ? 0 : ratio;
        }
    }
}